using LoanService.Models.Loan;

namespace LoanService.Services.Interfaces;

public interface ILoanManagerService
{
    Task<LoanPreviewDto> CalculateLoan(float amount, int termMonths, Guid rateId);
}