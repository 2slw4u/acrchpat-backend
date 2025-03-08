using CoreService.Models.Request.Account;
using CoreService.Models.Request.Support;
using CoreService.Models.Response.Account;
using CoreService.Models.Response.Support;
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
        public async Task<GetClientAccountsResponse> GetClientAccounts(GetClientAccountsRequest request)
        {
            return await _supportService.GetClientAccounts(HttpContext, request);
        }

        [HttpGet]
        [Route("account/{accountId}")]
        [EndpointSummary("(GetClientAccountDetails) Return detailed info about a certain account. Used by support specialists")]
        [Obsolete]
        [Authorize]
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
        public async Task ChangeClientAccountStatus(ChangeClientAccountStatusRequest request)
        {
            await _supportService.ChangeClientAccountStatus(HttpContext, request);
        }

        [HttpGet]
        [Route("transactions/{userId}")]
        [EndpointSummary("(GetClientTransactionHistory) Returns transaction history of a given user. Used by support specialists")]
        [Authorize]
        public async Task<GetClientTransactionHistoryResponse> GetClientTransactionHistory(GetClientTransactionHistoryRequest request)
        {
            return await _supportService.GetClientTransactionHistory(HttpContext, request);
        }
    }
}
