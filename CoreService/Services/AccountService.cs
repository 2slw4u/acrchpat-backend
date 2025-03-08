using AutoMapper;
using CoreService.Helpers;
using CoreService.Models.Database;
using CoreService.Models.Database.Entity;
using CoreService.Models.DTO;
using CoreService.Models.Exceptions;
using CoreService.Models.Request.Account;
using CoreService.Models.Response.Account;
using CoreService.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Cryptography;
using System.Security.Principal;

namespace CoreService.Services
{
    public class AccountService : IAccountService
    {
        private readonly CoreDbContext _dbContext;
        private readonly IMapper _mapper;
        public AccountService(CoreDbContext coreDbContext, IMapper mapper)
        {
            _dbContext = coreDbContext;
            _mapper = mapper;
        }
        public async Task ChangeAccountDetails(HttpContext httpContext, ChangeAccountDetailsRequest request)
        {
            throw new OperationNotImplemented();
        }

        public async Task CloseAccount(HttpContext httpContext, CloseAccountRequest request)
        {
            var userId = ContextDataHelper.GetUserId(httpContext);
            var account = await _dbContext.Accounts.Where(x => x.Id == request.accountId).FirstOrDefaultAsync();
            if (account == null)
            {
                throw new AccountNotFound();
            }
            else if (account.UserId != userId)
            {
                throw new UserDoesntOwnTheAccount();
            }
            else if (account.Status == Models.Enum.AccountStatus.Closed)
            {
                throw new AccountIsClosed();
            }
            else if (account.Balance != 0)
            {
                throw new AccountBalanceNotZero();
            }
            account.Status = Models.Enum.AccountStatus.Closed;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<GetAccountDetailsResponse> GetAccountDetails(HttpContext httpContext, GetAccountDetailsRequest request)
        {
            throw new OperationNotNeeded();
        }

        public async Task<GetAccountsResponse> GetAccounts(HttpContext httpContext)
        {
            var userId = ContextDataHelper.GetUserId(httpContext);
            var accounts = await _dbContext.Accounts.Where(x => x.UserId == userId).ToListAsync();
            return new GetAccountsResponse
            {
                Accounts = accounts.Select(x => _mapper.Map<AccountDTO>(x)).ToList()
            };
        }

        public async Task OpenNewAccount(HttpContext httpContext, OpenNewAccountRequest request)
        {
            var userId = ContextDataHelper.GetUserId(httpContext);
            var account = _mapper.Map<AccountEntity>(request.NewAccount);
            account.UserId = userId;
            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();
        }
    }
}
