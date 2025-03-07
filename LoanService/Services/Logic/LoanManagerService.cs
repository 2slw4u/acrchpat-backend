using LoanService.Database;
using LoanService.Models.Loan;
using LoanService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoanService.Services.Logic;

public class LoanManagerService : ILoanManagerService
{
    private readonly AppDbContext _dbContext;

    public LoanManagerService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LoanPreviewDto> CalculateLoan(float givenMoney, int termMonths, Guid rateId)
    {
        var rate = await _dbContext.Rates
            .FirstOrDefaultAsync(r => r.Id == rateId);

        if (rate == null)
        {
            throw new KeyNotFoundException($"Rate with ID {rateId} not found in the database");
        }
        
        float monthlyPayment = CalculateMonthlyPayment(rate.RateValue, givenMoney, termMonths);

        return new LoanPreviewDto()
        {
            totalAmount = monthlyPayment * termMonths,
            monthlyPayment = monthlyPayment
        };
    }

    public async Task<LoanDetailDto> GetLoan(Guid id)
    {
        var loan = await _dbContext.Loans
            .Include(l => l.Rate)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null)
        {
            throw new KeyNotFoundException($"Loan with ID {id} not found in the database");
        }
        
        var rate = await _dbContext.Rates
            .FirstOrDefaultAsync(r => r == loan.Rate);

        if (rate == null)
        {
            throw new KeyNotFoundException($"Rate with ID {loan.Rate.Id} not found in the database");
        }

        int termMonths = (loan.DeadlineTime - loan.CreateTime).Days; //потому что списываем каждый день
        
        float monthlyPayment = CalculateMonthlyPayment(rate.RateValue, loan.GivenMoney, termMonths);

        return new LoanDetailDto()
        {
            Id = id,
            UserId = loan.UserId,
            RateId = loan.Rate.Id,
            CreateTime = loan.CreateTime,
            DeadlineTime = loan.DeadlineTime,
            GivenMoney = loan.GivenMoney,
            Status = loan.Status,
            Transactions = loan.Transactions,
            TotalMoneyToPay = monthlyPayment * termMonths,
            MoneyLeftToPay = 0,
            MonthlyPayment = monthlyPayment,
            TermMonths = termMonths
        };
    }

    private float CalculateMonthlyPayment(float ratePercent, float givenMoney, int termMonths)
    {
        float monthlyRate = ratePercent / (12 * 100);

        return givenMoney * monthlyRate / (1 - (float)Math.Pow(1 + monthlyRate, -termMonths));
    }
}