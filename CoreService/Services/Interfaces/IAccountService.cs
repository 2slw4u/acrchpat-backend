using CoreService.Models.DTO;
using CoreService.Models.Request.Account;
using CoreService.Models.Response.Account;

namespace CoreService.Services.Interfaces
{
    public interface IAccountService
    {
        Task<GetAccountsResponse> GetAccounts(HttpContext httpContext);
        Task<GetAccountDetailsResponse> GetAccountDetails(HttpContext httpContext, GetAccountDetailsRequest request);
        Task OpenNewAccount (HttpContext httpContext, OpenNewAccountRequest request);
        Task CloseAccount (HttpContext httpContext, CloseAccountRequest request);
        Task ChangeAccountDetails(HttpContext httpContext, ChangeAccountDetailsRequest request);
    }
}
