using LoanService.Database;
using LoanService.Database.TableModels;
using LoanService.Exceptions;
using LoanService.Integrations;
using LoanService.Integrations.HttpRequesters;
using LoanService.Middleware;
using LoanService.Models.General;
using LoanService.Models.Loan;
using LoanService.Models.Rate;
using LoanService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoanService.Services.Logic;

public class LoanManagerService(AppDbContext dbContext,
    IRabbitMqTransactionRequestProducer producer,
    CoreRequester coreRequester,
    UserRequester userRequester
    ) : ILoanManagerService
{
    public async Task<LoanPreviewDto> CalculateLoan(double givenMoney, int termDays, Guid rateId)
    {
        var rate = await dbContext.Rates
            .FirstOrDefaultAsync(r => r.Id == rateId);

        if (rate == null)
        {
            throw new ResourceNotFoundException($"Rate with ID {rateId} not found in the database");
        }
        
        var dailyPayment = CalculateDailyPayment(rate.RateValue, givenMoney, termDays);

        return new LoanPreviewDto
        {
            TotalAmount = dailyPayment * termDays,
            DailyPayment = dailyPayment
        };
    }

    public async Task<LoanDetailDto> CreateLoan(Guid userId, LoanCreateModel model, List<RoleDto> roles)
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
        for (var i = 1; i <= model.TermDays; i++)
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
            Status = LoanStatus.Pending,
            GivenMoney = model.Amount,
            Transactions = new()
        };
        
        await dbContext.Loans.AddAsync(loan);
        await dbContext.SaveChangesAsync();
        
        await producer.SendTransactionRequestMessage(new TransactionRequest
        {
            LoanId = loan.Id,
            Amount = loan.GivenMoney,
            Type = TransactionType.LoanAccrual,
            AccountId = model.AccountIdToReceiveMoney,
            UserId = userId
        });
        
        return await GetLoan(loan.Id, userId, roles);
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
        
        var payedPaymentsCount = loan.Payments
            .Count(p => p.Status == PaymentStatus.Payed);
        
        var totalMoneyToPay = dailyPayment * termDays;
        var moneyLeftToPay = totalMoneyToPay - dailyPayment * payedPaymentsCount;
        
        var transactions = new GetTransactionsDataResponse();

        if (loan.Transactions.Count > 0)
        {
            transactions = await coreRequester.GetTransactionsAsync(loan.Transactions);
            //Console.WriteLine($"got transactions: {transactions.Transactions.Count}");
        }
        
        return new LoanDetailDto
        {
            Id = id,
            UserId = loan.UserId,
            Rate = new RateDto
            {
                Id = loan.Rate.Id,
                Name = loan.Rate.Name,
                YearlyRate = loan.Rate.RateValue
            },
            CreateTime = loan.CreateTime,
            DeadlineTime = loan.DeadlineTime,
            GivenMoney = loan.GivenMoney,
            Status = loan.Status,
            Transactions = transactions.Transactions,
            TotalMoneyToPay = totalMoneyToPay,
            MoneyLeftToPay = moneyLeftToPay,
            DailyPayment = dailyPayment,
            TermDays = termDays,
            Payments = loan.Payments
                .OrderBy(p => p.PaymentTime)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PaymentTime = p.PaymentTime,
                    Status = p.Status
                }).ToList()
        };
    }

    public async Task<string> PayLoan(Guid userId, Guid loanId, Guid? paymentId, Guid? accountId)
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

        if (loan.Status == LoanStatus.Closed)
        {
            throw new BadRequestException($"Loan {loanId} is closed and does not require additional payments");
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

            if (payment.Status == PaymentStatus.Payed)
            {
                throw new BadRequestException($"Loan payment with ID {paymentId} has already been done");
            }
        }
        else
        {
            payment = loan.Payments
                .Where(p => p.Status != PaymentStatus.Payed)
                .MinBy(p => p.PaymentTime);

            if (payment == null)
            {
                loan.Status = LoanStatus.Closed;
                await dbContext.SaveChangesAsync();
                throw new ConflictException($"Loan {loanId} has no payments that need to be done. Now the loan is marked as closed.");
            }
        }
        
        await producer.SendTransactionRequestMessage(new TransactionRequest
        {
            LoanId = loan.Id,
            PaymentId = payment.Id,
            Amount = payment.Amount,
            Type = TransactionType.LoanPayment,
            UserId = userId,
            AccountId = accountId
        });

        return $"Transaction request for payment {payment.Id} of loan {loan.Id} has been sent";
    }

    public async Task<List<LoanShortDto>> GetLoanHistory(Guid userId)
    {
        var user = await userRequester.GetUserAsync(userId);
        if (user.Roles.All(r => r.Name != "Client"))
        {
            throw new BadRequestException($"User {userId} must be a Client");
        }
        
        return await dbContext.Loans
            .Where(l => l.UserId == userId)
            .Select(l => new LoanShortDto
            {
                Id = l.Id,
                Amount = l.GivenMoney,
                DeadlineTime = l.DeadlineTime,
                Rate = l.Rate.RateValue,
                Status = l.Status
            })
            .ToListAsync();
    }

    public async Task<float> CalculateLoanRating(Guid userId)
    {
        var user = await userRequester.GetUserAsync(userId);
        if (user.Roles.All(r => r.Name != "Client"))
        {
            throw new BadRequestException($"User {userId} must be a Client");
        }
        
        var loans = await dbContext.Loans
            .Where(l => l.UserId == userId)
            .ToListAsync();
        
        var pendingLoans = loans.Count(l => l.Status == LoanStatus.Pending);
        var totalLoans = loans.Count - pendingLoans;
        if (loans.Count == 0)
            return 1f;
        
        var closedLoans = loans.Count(l => l.Status == LoanStatus.Closed);
        var overdueLoans = loans.Count(l => l.Status == LoanStatus.Overdue);
        var openLoans = loans.Count(l => l.Status == LoanStatus.Open);
        
        var score = closedLoans * 1.0f + openLoans * 0.5f - overdueLoans * 1.0f;
        return score / totalLoans;
    }

    public async Task AddTransaction(Guid loanId, Guid transactionId, Guid? paymentId)
    {
        var loan = await dbContext.Loans
            .Include(l => l.Payments)
            .FirstOrDefaultAsync(l => l.Id == loanId);

        if (loan == null)
        {
            throw new ResourceNotFoundException($"Loan with ID {loanId} not found in database");
        }

        if (loan.Transactions.Count == 0 && loan.Status == LoanStatus.Pending)
        {
            loan.Status = LoanStatus.Open;
        }
        
        loan.Transactions.Add(transactionId);

        if (paymentId != Guid.Empty)
        {
            var payment = loan.Payments
                .FirstOrDefault(p => p.Id == paymentId);

            if (payment == null)
            {
                throw new ResourceNotFoundException($"Loan with ID {loanId} does not have a payment with ID {paymentId}");
            }

            payment.Status = PaymentStatus.Payed;

            if (loan.Payments.Count(p => p.Status != PaymentStatus.Payed) == 0)
            {
                loan.Status = LoanStatus.Closed;
            }
        }
        
        await dbContext.SaveChangesAsync();
    }

    public async Task MarkPaymentAsOverdue(Guid loanId, Guid paymentId)
    {
        var loan = await dbContext.Loans
            .Include(l => l.Payments)
            .FirstOrDefaultAsync(l => l.Id == loanId);

        if (loan == null)
        {
            throw new ResourceNotFoundException($"Loan with ID {loanId} not found in database");
        }
        
        var payment = loan.Payments
            .FirstOrDefault(p => p.Id == paymentId);

        if (payment == null)
        {
            throw new ResourceNotFoundException($"Loan with ID {loanId} does not have a payment with ID {paymentId}");
        }

        if (payment.PaymentTime >= DateTime.Now)
        {
            payment.Status = PaymentStatus.Overdue;

            if (loan.Payments.Count(p => p.Status == PaymentStatus.NotYet) == 0)
            {
                loan.Status = LoanStatus.Overdue;
            }
        
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteInvalidLoan(Guid loanId)
    {
        var loan = await dbContext.Loans
            .FirstOrDefaultAsync(l => l.Id == loanId);

        if (loan == null)
        {
            throw new ResourceNotFoundException($"Loan with ID {loanId} not found in database");
        }

        dbContext.Remove(loan);
        await dbContext.SaveChangesAsync();
    }

    private static double CalculateDailyPayment(double ratePercent, double givenMoney, int termDays)
    {
        var dailyRate = ratePercent / (365 * 100);

        return givenMoney * dailyRate / (1 - Math.Pow(1 + dailyRate, -termDays));
    }
}