using CoreService.Attributes;
using CoreService.Models.Enum;
using CoreService.Models.Http.Request.Support;
using CoreService.Models.Http.Response.Support;
using CoreService.Services;
using CoreService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreService.Controllers
{
    [ApiController]
    [Route("core/support")]
    public class SupportController : Controller
    {
        private readonly ISupportService _supportService;
        public SupportController(ISupportService supportService)
        {
            _supportService = supportService;
        }
        
        [HttpGet]
        [Route("account")]
        [EndpointSummary("(GetClientAccounts) Return all accounts owned by specified users. Used by support specialists")]
        [Authorize]
        [RoleAuthorize(UserRole.Employee)]
        public async Task<GetClientAccountsResponse> GetClientAccounts(GetClientAccountsRequest request)
        {
            return await _supportService.GetClientAccounts(HttpContext, request);
        }

        [HttpGet]
        [Route("account/{accountId}")]
        [EndpointSummary("(GetClientAccountDetails) Return detailed info about a certain account. Used by support specialists")]
        [Obsolete]
        [Authorize]
        [RoleAuthorize(UserRole.Employee)]
        public async Task<GetClientAccountDetailsResponse> GetClientAccountDetails(GetClientAccountDetailsRequest request)
        {
            return await _supportService.GetClientAccountDetails(HttpContext, request);
        }

        [HttpPut]
        [Route("account/{accountId}/status")]
        [EndpointSummary("(ChangeClientAccountStatus) Changes a given account status. Used by support specialists")]
        [EndpointDescription("This one is fully optional and will likely not be implemented in MVP")]
        [Obsolete]
        [Authorize]
        [RoleAuthorize(UserRole.Employee)]
        public async Task ChangeClientAccountStatus(ChangeClientAccountStatusRequest request)
        {
            await _supportService.ChangeClientAccountStatus(HttpContext, request);
        }

        [HttpGet]
        [Route("transactions/{accountId}")]
        [EndpointSummary("(GetAccountTransactionHistory) Returns transaction history of a given account. Used by support specialists")]
        [Authorize]
        [RoleAuthorize(UserRole.Employee)]
        public async Task<GetAccountTransactionHistoryResponse> GetAccountTransactionHistory(GetAccountTransactionHistoryRequest request)
        {
            return await _supportService.GetAccountTransactionHistory(HttpContext, request);
        }
    }
}
