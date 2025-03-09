using AutoMapper;
using CoreService.Helpers;
using CoreService.Models.Database;
using CoreService.Models.Database.Entity;
using CoreService.Models.DTO;
using CoreService.Models.Enum;
using CoreService.Models.Exceptions;
using CoreService.Models.Http.Request.Transaction;
using CoreService.Models.Http.Response.Transaction;
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

        public async Task<DepositMoneyToAccountResponse> DepositMoneyToAccount(HttpContext httpContext, DepositMoneyToAccountRequest request)
        {
            var userId = ContextDataHelper.GetUserId(httpContext);
            var account = _dbContext.Accounts.Where(x => x.Id == request.accountId).FirstOrDefault();
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
            account.Balance += request.Deposit.Amount;
            var transaction = _mapper.Map<TransactionEntity>(request);
            transaction.Account = account;
            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
            return new DepositMoneyToAccountResponse
            {
                NewDepositTransaction = _mapper.Map<TransactionDTO>(transaction)
            };
        }

        public async Task<GetTransactionsDataResponse> GetTransactionsData(HttpContext httpContext, GetTransactionsDataRequest request)
        {
            if (request.Transactions == null || request.Transactions.Count == 0)
            {
                return new GetTransactionsDataResponse();
            }
            var userId = ContextDataHelper.GetUserId(httpContext);
            var transactions = _dbContext.Transactions
                .Where(x => request.Transactions.Contains(x.Id))
                .OrderByDescending(x => x.PerformedAt);
            if (request.Transactions.Any(x => !transactions.Select(y => y.Id).Contains(x)))
            {
                throw new TransactionNotFound();
            }
            else if (transactions.Any(x => x.Account.UserId != userId))
            {
                throw new UserDoesntOwnTheAccount();
            }
            return new GetTransactionsDataResponse
            {
                Transactions = await transactions.Select(x => _mapper.Map<TransactionDTO>(x)).ToListAsync()
            };
        }

        public async Task<GetTransactionsHistoryResponse> GetTransactionsHistory(HttpContext httpContext, GetTransactionsHistoryRequest request)
        {
            var userId = ContextDataHelper.GetUserId(httpContext);
            var accounts = _dbContext.Accounts.Where(x => x.UserId == userId);
            if (request.Accounts != null  && request.Accounts.Count > 0)
            {
                accounts = accounts.Where(x => request.Accounts.Contains(x.Id));
                if (request.Accounts.Any(x => !accounts.Select(y => y.Id).Contains(x)))
                {
                    throw new AccountNotFound();
                }
                else if (accounts.Any(x => x.UserId != userId))
                {
                    throw new UserDoesntOwnTheAccount();
                }
            } 
            var transactions = await _dbContext.Transactions
                .Where(x => (accounts.Select(y => y.Id).Contains(x.Account.Id)))
                .OrderByDescending(x => x.PerformedAt)
                .ToListAsync();
            return new GetTransactionsHistoryResponse
            {
                Transactions = transactions.Select(x => _mapper.Map<TransactionDTO>(x)).ToList()
            };
        }

        public async Task<WithdrawMoneyFromAccountResponse> WithdrawMoneyFromAccount(HttpContext httpContext, WithdrawMoneyFromAccountRequest request)
        {
            var userId = ContextDataHelper.GetUserId(httpContext);
            var account = _dbContext.Accounts.Where(x => x.Id == request.accountId).FirstOrDefault();
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
            else if (account.Balance < request.Withdrawal.Amount)
            {
                throw new NotEnoughMoney();
            }
            account.Balance -= request.Withdrawal.Amount;
            var transaction = _mapper.Map<TransactionEntity>(request);
            transaction.Account = account;
            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
            return new WithdrawMoneyFromAccountResponse
            {
                NewWithdrawalTransaction = _mapper.Map<TransactionDTO>(transaction)
            };
        }
    }
}
