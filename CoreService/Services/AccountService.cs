using AutoMapper;
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
        private readonly IMemoryCache _memoryCache;
        private readonly CoreDbContext _dbContext;
        private readonly IMapper _mapper;
        public AccountService(IMemoryCache memoryCache, CoreDbContext coreDbContext, IMapper mapper)
        {
            _memoryCache = memoryCache;
            _dbContext = coreDbContext;
            _mapper = mapper;
        }
        public async Task ChangeAccountDetails(HttpContext httpContext, ChangeAccountDetailsRequest request)
        {
            throw new OperationNotImplemented();
        }

        public async Task CloseAccount(HttpContext httpContext, CloseAccountRequest request)
        {
            var account = await _dbContext.Accounts.Where(x => x.Id == request.accountId).FirstOrDefaultAsync();
            if (account == null)
            {
                throw new AccountNotFound();
            }
            else if (account.Status == Models.Enum.AccountStatus.Closed)
            {
                throw new AccountIsClosed();
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
            var userId = (Guid)httpContext.Items["UserId"];
            var accounts = await _dbContext.Accounts.Where(x => x.UserId == userId).ToListAsync();
            return new GetAccountsResponse
            {
                Accounts = accounts.Select(x => _mapper.Map<AccountDTO>(x)).ToList()
            };
        }

        public async Task OpenNewAccount(HttpContext httpContext, OpenNewAccountRequest request)
        {
            var account = _mapper.Map<AccountEntity>(request.NewAccount);
            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();
        }
    }
}
