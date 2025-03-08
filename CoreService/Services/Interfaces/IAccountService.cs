using CoreService.Models.DTO;
using CoreService.Models.Http.Request.Account;
using CoreService.Models.Http.Response.Account;

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
