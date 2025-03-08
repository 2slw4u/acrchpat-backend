using CoreService.Models.Http.Request.Support;
using CoreService.Models.Http.Response.Support;

namespace CoreService.Services.Interfaces
{
    public interface ISupportService
    {
        Task<GetClientAccountsResponse> GetClientAccounts(HttpContext httpContext, GetClientAccountsRequest request);
        Task<GetClientAccountDetailsResponse> GetClientAccountDetails(HttpContext httpContext, GetClientAccountDetailsRequest request);
        Task<GetClientTransactionHistoryResponse> GetClientTransactionHistory(HttpContext httpContext, GetClientTransactionHistoryRequest request);
        Task ChangeClientAccountStatus(HttpContext httpContext, ChangeClientAccountStatusRequest request);
    }
}
