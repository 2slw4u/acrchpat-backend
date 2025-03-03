using CoreService.Models.DTO;
using CoreService.Models.Request.Account;
using CoreService.Models.Response.Account;
using CoreService.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace CoreService.Controllers
{
    [ApiController]
    [Route("core/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        
        [HttpGet]
        [EndpointSummary("(GetAccounts) Return all accounts owned by user")]
        public async Task<GetAccountsResponse> GetAccounts(GetAccountsRequest request)
        {
            return await _accountService.GetAccounts(HttpContext, request);
        }
        
        [HttpPost]
        [EndpointSummary("(OpenNewAccount) Opens new account for a given user")]
        public async Task OpenNewAccount(OpenNewAccountRequest request)
        {
            await _accountService.OpenNewAccount(HttpContext, request);
        }

        [HttpGet]
        [Route("{accountId}")]
        [EndpointSummary("(GetAccountDetails) Returns a detailed representation of an account")]
        [EndpointDescription("This one is fully optional and will likely not be implemented in MVP")]
        [Obsolete]
        public async Task<GetAccountDetailsResponse> GetAccountDetails(GetAccountDetailsRequest request)
        {
            return await _accountService.GetAccountDetails(HttpContext, request);
        }

        [HttpDelete]
        [Route("{accountId}")]
        [EndpointSummary("(CloseAccount) Closes account of a user")]
        public async Task CloseAccount(CloseAccountRequest request)
        {
            await _accountService.CloseAccount(HttpContext, request);
        }

        [HttpPatch]
        [Route("{accountId}")]
        [EndpointSummary("(ChangeAccountDetails) Changes account details")]
        [EndpointDescription("This one is fully optional and will likely not be implemented in MVP")]
        [Obsolete]
        public async Task ChangeAccountDetails(ChangeAccountDetailsRequest request)
        {
            await _accountService.ChangeAccountDetails(HttpContext, request);
        }

    }
}
