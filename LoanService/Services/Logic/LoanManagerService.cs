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

    public async Task<LoanPreviewDto> CalculateLoan(float amount, int termMonths, Guid rateId)
    {
        var rate = await _dbContext.Rates
            .FirstOrDefaultAsync(r => r.Id == rateId);

        if (rate == null)
        {
            throw new KeyNotFoundException($"Rate with ID {rateId} not found in the database");
        }

        float ratePercent = rate.RateValue;

        float monthlyRate = ratePercent / (12 * 100);

        float monthlyPayment = amount * monthlyRate / (1 - (float)Math.Pow(1 + monthlyRate, -termMonths));

        return new LoanPreviewDto()
        {
            totalAmount = monthlyPayment * termMonths,
            monthlyPayment = monthlyPayment
        };
    }
}