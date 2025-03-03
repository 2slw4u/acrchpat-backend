﻿using AutoMapper;
using CoreService.Models.Database;
using CoreService.Models.Database.Entity;
using CoreService.Models.DTO;
using CoreService.Models.Request.Account;
using CoreService.Models.Response.Account;
using CoreService.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
            throw new NotImplementedException();
        }

        public async Task CloseAccount(HttpContext httpContext, CloseAccountRequest request)
        {
            var account = await _dbContext.Accounts.Where(x => x.Id == request.accountId).FirstOrDefaultAsync();
            if (account == null)
            {
                throw new BadHttpRequestException("Account is not found", 404);
            }
            else if (account.Status == Models.Enum.AccountStatus.Closed)
            {
                throw new BadHttpRequestException("Account is already closed", 422);
            }
            account.Status = Models.Enum.AccountStatus.Closed;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<GetAccountDetailsResponse> GetAccountDetails(HttpContext httpContext, GetAccountDetailsRequest request)
        {
            var account = new AccountDTO
            {
                Id = request.accountId,
                Status = Models.Enum.AccountStatus.Opened,
                Balance = 0.2,
                Name = "testing"
            };
            var response = new GetAccountDetailsResponse
            {
                Account = account
            };
            if (_memoryCache.TryGetValue<AccountDTO>(request.accountId, out var result))
            {
                throw new KeyNotFoundException("This is in cache already" + $"\n{result.Name}");
            }
            else
            {
                _memoryCache.Set<AccountDTO>(request.accountId, account, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20)
                });
                throw new NotImplementedException();
            }
        }

        public async Task<GetAccountsResponse> GetAccounts(HttpContext httpContext, GetAccountsRequest request)
        {
            var accounts = await _dbContext.Accounts.Where(x => x.UserId == request.user_id).ToListAsync();
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
