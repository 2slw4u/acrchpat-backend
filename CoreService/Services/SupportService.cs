using AutoMapper;
using CoreService.Models.Database;
using CoreService.Models.Database.Entity;
using CoreService.Models.DTO;
using CoreService.Models.Exceptions;
using CoreService.Models.Http.Request.Support;
using CoreService.Models.Http.Response.Support;
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
            throw new OperationNotNeeded();
        }

        public async Task<GetClientAccountDetailsResponse> GetClientAccountDetails(HttpContext httpContext, GetClientAccountDetailsRequest request)
        {
            throw new OperationNotNeeded();
        }

        public async Task<GetClientAccountsResponse> GetClientAccounts(HttpContext httpContext, GetClientAccountsRequest request)
        {
            var accounts = await _dbContext.Accounts.Where(x => request.Users.Contains(x.Id)).ToListAsync();
            return new GetClientAccountsResponse
            {
                Accounts = accounts.Select(x => _mapper.Map<DetailedAccountDTO>(x)).ToList()
            };
        }

        public async Task<GetClientTransactionHistoryResponse> GetClientTransactionHistory(HttpContext httpContext, GetClientTransactionHistoryRequest request)
        {
            var transactions = await _dbContext.Transactions.Where(x => x.Account.UserId == request.userId).ToListAsync();
            return new GetClientTransactionHistoryResponse
            {
                Transactions = transactions.Select(x => _mapper.Map<DetailedTransactionDTO>(x)).ToList()
            };
        }
    }
}
