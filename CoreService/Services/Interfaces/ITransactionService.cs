using CoreService.Models.Http.Request.Transaction;
using CoreService.Models.Http.Response.Transaction;

namespace CoreService.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<GetTransactionsHistoryResponse> GetTransactionsHistory(HttpContext httpContext, GetTransactionsHistoryRequest request); 
        Task DepositMoneyToAccount(HttpContext httpContext, DepositMoneyToAccountRequest request);
        Task WithdrawMoneyFromAccount(HttpContext httpContext, WithdrawMoneyFromAccountRequest request);
        Task TransferMoneyToAccount(HttpContext httpContext, TransferMoneyToAccountRequest request);
        Task<GetTransferMoneyRatesResponse> GetTransferMoneyRates(HttpContext httpContext, GetTransferMoneyRatesRequest request);
        Task<GetTransactionsDataResponse> GetTransactionsData(HttpContext httpContext, GetTransactionsDataRequest request);
    }
}
