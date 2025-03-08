using CoreService.Attributes;
using CoreService.Models.DTO;
using CoreService.Models.Enum;
using CoreService.Models.Http.Request.Account;
using CoreService.Models.Http.Response.Account;
using CoreService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [RoleAuthorize(UserRole.Client)]
        public async Task<GetAccountsResponse> GetAccounts()
        {
            return await _accountService.GetAccounts(HttpContext);
        }
        
        [HttpPost]
        [EndpointSummary("(OpenNewAccount) Opens new account for a given user")]
        [Authorize]
        [RoleAuthorize(UserRole.Client)]
        public async Task OpenNewAccount(OpenNewAccountRequest request)
        {
            await _accountService.OpenNewAccount(HttpContext, request);
        }

        [HttpGet]
        [Route("{accountId}")]
        [EndpointSummary("(GetAccountDetails) Returns a detailed representation of an account")]
        [EndpointDescription("This one is fully optional and will likely not be implemented in MVP")]
        [Obsolete]
        [Authorize]
        [RoleAuthorize(UserRole.Client)]
        public async Task<GetAccountDetailsResponse> GetAccountDetails(GetAccountDetailsRequest request)
        {
            return await _accountService.GetAccountDetails(HttpContext, request);
        }

        [HttpDelete]
        [Route("{accountId}")]
        [EndpointSummary("(CloseAccount) Closes account of a user")]
        [Authorize]
        [RoleAuthorize(UserRole.Client)]
        public async Task CloseAccount(CloseAccountRequest request)
        {
            await _accountService.CloseAccount(HttpContext, request);
        }

        [HttpPatch]
        [Route("{accountId}")]
        [EndpointSummary("(ChangeAccountDetails) Changes account details")]
        [EndpointDescription("This one is fully optional and will likely not be implemented in MVP")]
        [Obsolete]
        [Authorize]
        [RoleAuthorize(UserRole.Client)]
        public async Task ChangeAccountDetails(ChangeAccountDetailsRequest request)
        {
            await _accountService.ChangeAccountDetails(HttpContext, request);
        }

    }
}
