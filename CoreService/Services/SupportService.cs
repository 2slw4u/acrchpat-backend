﻿using AutoMapper;
using CoreService.Models.Database;
using CoreService.Models.DTO;
using CoreService.Models.Request.Support;
using CoreService.Models.Response.Support;
using CoreService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Services
{
    public class SupportService : ISupportService
    {
        private readonly CoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public SupportService(CoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task ChangeClientAccountStatus(HttpContext httpContext, ChangeClientAccountStatusRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<GetClientAccountDetailsResponse> GetClientAccountDetails(HttpContext httpContext, GetClientAccountDetailsRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<GetClientAccountsResponse> GetClientAccounts(HttpContext httpContext, GetClientAccountsRequest request)
        {
            if (request.Users.Count == 0)
            {
                throw new BadHttpRequestException("No users specified", 422);
            }
            var accounts = await _dbContext.Accounts.Where(x => request.Users.Contains(x.Id)).ToListAsync();
            return new GetClientAccountsResponse
            {
                Accounts = accounts.Select(x => _mapper.Map<DetailedAccountDTO>(x)).ToList()
            };
        }

        public async Task<GetClientTransactionHistoryResponse> GetClientTransactionHistory(HttpContext httpContext, GetClientTransactionHistoryRequest request)
        {
            var transactions = await _dbContext.Accounts.Where(x => x.Id == request.userId).ToListAsync();
            return new GetClientTransactionHistoryResponse
            {
                Transactions = transactions.Select(x => _mapper.Map<DetailedTransactionDTO>(x)).ToList()
            };
        }
    }
}
