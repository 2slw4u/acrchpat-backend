using LoanService.Database;
using LoanService.Database.TableModels;
using LoanService.Exceptions;
using LoanService.Middleware;
using LoanService.Models.General;
using LoanService.Models.Loan;
using LoanService.Models.Rate;
using LoanService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoanService.Services.Logic;

public class LoanManagerService(AppDbContext dbContext) : ILoanManagerService
{
    public async Task<LoanPreviewDto> CalculateLoan(double givenMoney, int termDays, Guid rateId)
    {
        var rate = await dbContext.Rates
            .FirstOrDefaultAsync(r => r.Id == rateId);

        if (rate == null)
        {
            throw new ResourceNotFoundException($"Rate with ID {rateId} not found in the database");
        }
        
        double dailyPayment = CalculateDailyPayment(rate.RateValue, givenMoney, termDays);

        return new LoanPreviewDto
        {
            TotalAmount = dailyPayment * termDays,
            DailyPayment = dailyPayment
        };
    }

    public async Task<Guid> CreateLoan(Guid userId, LoanCreateModel model)
    {
        var deadlineTime = DateTime.UtcNow.AddDays(model.TermDays);
        
        var rate = await dbContext.Rates
            .FirstOrDefaultAsync(r => r.Id == model.RateId);

        if (rate == null)
        {
            throw new ResourceNotFoundException($"Rate with ID {model.RateId} not found in the database");
        }
        
        var dailyPayment = CalculateDailyPayment(rate.RateValue, model.Amount, model.TermDays);
        
        var payments = new List<LoanPayment>();
        for (int i = 1; i <= model.TermDays; i++)
        {
            payments.Add(new LoanPayment
            {
                Id = Guid.NewGuid(),
                PaymentTime = DateTime.UtcNow.AddDays(i),
                Status = PaymentStatus.NotYet,
                Amount = dailyPayment
            });
        }
        
        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow,
            DeadlineTime = deadlineTime,
            UserId = userId,
            Payments = payments,
            Rate = rate,
            Status = LoanStatus.Open,
            GivenMoney = model.Amount,
            Transactions = new()
        };
        
        //отправить запрос на зачисление денег в кролика
        
        await dbContext.Loans.AddAsync(loan);
        await dbContext.SaveChangesAsync();

        return loan.Id;
    }

    public async Task<LoanDetailDto> GetLoan(Guid id, Guid userId, List<RoleDto> roles)
    {
        var loan = await dbContext.Loans
            .Include(l => l.Rate)
            .Include(l => l.Payments)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null)
        {
            throw new ResourceNotFoundException($"Loan with ID {id} not found in the database");
        }

        if (loan.UserId != userId && roles.All(r => r.Name != "Employee"))
        {
            throw new AccessDeniedException($"You do not have access to the loan with ID {id}");
        }

        var termDays = (loan.DeadlineTime - loan.CreateTime).Days;
        
        var dailyPayment = CalculateDailyPayment(loan.Rate.RateValue, loan.GivenMoney, termDays);
        
        //получить список транзакций по кредиту
        var transactions = new List<TransactionDto>();

        return new LoanDetailDto
        {
            Id = id,
            UserId = loan.UserId,
            Rate = new RateDto
            {
                Id = loan.Rate.Id,
                Name = loan.Rate.Name,
                TwelveDayRate = loan.Rate.RateValue
            },
            CreateTime = loan.CreateTime,
            DeadlineTime = loan.DeadlineTime,
            GivenMoney = loan.GivenMoney,
            Status = loan.Status,
            Transactions = transactions,
            TotalMoneyToPay = dailyPayment * termDays,
            MoneyLeftToPay = 0, //надо посчитать
            DailyPayment = dailyPayment,
            TermDays = termDays,
            Payments = loan.Payments
                .Select(p => new PaymentDto()
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PaymentTime = p.PaymentTime,
                    Status = p.Status
                }).ToList()
        };
    }

    public async Task<int> PayLoan(Guid userId, Guid loanId, Guid? paymentId)
    {
        var loan = await dbContext.Loans
            .Include(l => l.Payments)
            .FirstOrDefaultAsync(l => l.Id == loanId);

        if (loan == null)
        {
            throw new ResourceNotFoundException($"Loan with ID {loanId} not found in the database");
        }
        
        if (loan.UserId != userId)
        {
            throw new AccessDeniedException($"You do not have access to the loan with ID {loanId}");
        }

        LoanPayment? payment;

        if (paymentId != null)
        {
            payment = loan.Payments
                .FirstOrDefault(p => p.Id == paymentId);
            
            if (payment == null)
            {
                throw new ResourceNotFoundException($"Loan with ID {loanId} does not have a payment with ID {paymentId}");
            }
        }
        else
        {
            payment = loan.Payments
                .Where(p => p.PaymentTime > DateTime.UtcNow)
                .MinBy(p => p.PaymentTime);

            if (payment == null)
            {
                throw new ResourceNotFoundException($"Loan with ID {loanId} does not have any payments associated with it for some reason...");
            }
        }
        
        //оплатить

        return 0; //возвращаю дто транзакции (типа чек) или ошибку
    }

    public async Task<List<LoanShortDto>> GetLoanHistory(Guid userId)
    {
        return await dbContext.Loans
            .Where(l => l.UserId == userId)
            .Select(l => new LoanShortDto()
            {
                Id = l.Id,
                Amount = l.GivenMoney,
                DeadlineTime = l.DeadlineTime,
                Rate = l.Rate.RateValue,
                Status = l.Status
            })
            .ToListAsync();
    }

    private static double CalculateDailyPayment(double ratePercent, double givenMoney, int termDays)
    {
        var dailyRate = ratePercent / (365 * 100);

        return givenMoney * dailyRate / (1 - (double)Math.Pow(1 + dailyRate, -termDays));
    }
}