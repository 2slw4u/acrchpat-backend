using LoanService.Middleware;
using LoanService.Models.Loan;

namespace LoanService.Services.Interfaces;

public interface ILoanManagerService
{
    Task<LoanPreviewDto> CalculateLoan(double givenMoney, int termDays, Guid rateId);

    Task<Guid> CreateLoan(Guid userId, LoanCreateModel model);

    Task<LoanDetailDto> GetLoan(Guid id, Guid userId, List<RoleDto> roles);

    Task<int> PayLoan(Guid userId, Guid loanId, Guid? paymentId);

    Task<List<LoanShortDto>> GetLoanHistory(Guid userId);
}