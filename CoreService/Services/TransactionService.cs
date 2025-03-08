using AutoMapper;
using CoreService.Models.Database;
using CoreService.Models.Database.Entity;
using CoreService.Models.DTO;
using CoreService.Models.Exceptions;
using CoreService.Models.Request.Transaction;
using CoreService.Models.Response.Transaction;
using CoreService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly CoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public TransactionService(CoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task DepositMoneyToAccount(HttpContext httpContext, DepositMoneyToAccountRequest request)
        {
            var account = _dbContext.Accounts.Where(x => x.Id == request.accountId).FirstOrDefault();
            if (account == null)
            {
                throw new AccountNotFound();
            }
            else if (account.Status == Models.Enum.AccountStatus.Closed)
            {
                throw new AccountIsClosed();
            }
            account.Balance += request.Deposit.Amount;
            var transaction = _mapper.Map<TransactionEntity>(request);
            transaction.Account = account;
            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<GetTransactionsHistoryResponse> GetTransactionsHistory(HttpContext httpContext, GetTransactionsHistoryRequest request)
        {
            var transactions = _dbContext.Transactions
                .Where(x => x.Account.UserId == request.user_id);
            if (request.Accounts != null && request.Accounts.Count > 0)
            {
                transactions = transactions.Where(x => (request.Accounts.Contains(x.Account.Id)));
            }
            var orderedTransactions = await transactions.OrderByDescending(x => x.PerformedAt).ToListAsync();
            return new GetTransactionsHistoryResponse
            {
                Transactions = orderedTransactions.Select(x => _mapper.Map<TransactionDTO>(x)).ToList()
            };
        }

        public async Task WithdrawMoneyFromAccount(HttpContext httpContext, WithdrawMoneyFromAccountRequest request)
        {
            var account = _dbContext.Accounts.Where(x => x.Id == request.accountId).FirstOrDefault();
            if (account == null)
            {
                throw new AccountNotFound();
            }
            else if (account.Status == Models.Enum.AccountStatus.Closed)
            {
                throw new NotEnoughMoney();
            }
            else if (account.Balance < request.Withdrawal.Amount)
            {
                throw new BadHttpRequestException("There is not enough money in account", 422);
            }
            account.Balance -= request.Withdrawal.Amount;
            var transaction = _mapper.Map<TransactionEntity>(request);
            transaction.Account = account;
            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
        }
    }
}
