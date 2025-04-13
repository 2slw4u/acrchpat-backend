using LoanService.Middleware;
using LoanService.Models.Loan;

namespace LoanService.Services.Interfaces;

public interface ILoanManagerService
{
    Task<LoanPreviewDto> CalculateLoan(double givenMoney, int termDays, Guid rateId);

    Task<LoanDetailDto> CreateLoan(Guid userId, LoanCreateModel model, List<RoleDto> roles);

    Task<LoanDetailDto> GetLoan(Guid id, Guid userId, List<RoleDto> roles);

    Task<string> PayLoan(Guid userId, Guid loanId, Guid? paymentId, Guid? accountId);

    Task<List<LoanShortDto>> GetLoanHistory(Guid userId);

    Task AddTransaction(Guid loanId, Guid transactionId, Guid? paymentId);

    Task MarkPaymentAsOverdue(Guid loanId, Guid paymentId);

    Task DeleteInvalidLoan(Guid loanId);
}