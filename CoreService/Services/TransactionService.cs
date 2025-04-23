using AutoMapper;
using CoreService.Helpers;
using CoreService.Integrations.AMQP.RabbitMQ.Producer;
using CoreService.Integrations.Http.UniRate;
using CoreService.Models.Database;
using CoreService.Models.Database.Entity;
using CoreService.Models.DTO;
using CoreService.Models.Enum;
using CoreService.Models.Exceptions;
using CoreService.Models.Http.Request.Transaction;
using CoreService.Models.Http.Request.UniRate;
using CoreService.Models.Http.Response.Transaction;
using CoreService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly CoreDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IRabbitMqProducer _rabbitmqProducer;
        private readonly IUniRateAdapter _uniRateAdapter;

        public TransactionService(CoreDbContext dbContext, IMapper mapper, IRabbitMqProducer rabbitMqProducer, IUniRateAdapter uniRateAdapter)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _rabbitmqProducer = rabbitMqProducer;
            _uniRateAdapter = uniRateAdapter;
        }

        public async Task<GetTransactionsHistoryResponse> GetTransactionsHistory(HttpContext httpContext, GetTransactionsHistoryRequest request)
        {
            var userId = ContextDataHelper.GetUserId(httpContext);
            var accounts = _dbContext.Accounts.Where(x => x.UserId == userId);
            if (request.Accounts != null && request.Accounts.Count > 0)
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

        public async Task DepositMoneyToAccount(HttpContext httpContext, DepositMoneyToAccountRequest request)
        {
            var userId = ContextDataHelper.GetUserId(httpContext);
            var account = _dbContext.Accounts.Where(x => x.Id == request.accountId).FirstOrDefault();
            if (account == null)
            {
                throw new AccountNotFound();
            }
            if (account.UserId != userId)
            {
                throw new UserDoesntOwnTheAccount();
            }
            if (account.Status == Models.Enum.AccountStatus.Closed)
            {
                throw new AccountIsClosed();
            }
            await _rabbitmqProducer.SendTransactionRequestMessage(new TransactionRequestDTO
            {
                UserId = userId,
                AccountId = account.Id,
                Amount = request.Deposit.Amount,
                Type = TransactionType.Deposit
            });
        }

        public async Task WithdrawMoneyFromAccount(HttpContext httpContext, WithdrawMoneyFromAccountRequest request)
        {
            var userId = ContextDataHelper.GetUserId(httpContext);
            var account = _dbContext.Accounts.Where(x => x.Id == request.accountId).FirstOrDefault();
            if (account == null)
            {
                throw new AccountNotFound();
            }
            if (account.UserId != userId)
            {
                throw new UserDoesntOwnTheAccount();
            }
            if (account.Status == Models.Enum.AccountStatus.Closed)
            {
                throw new AccountIsClosed();
            }
            if (account.Balance < request.Withdrawal.Amount)
            {
                throw new NotEnoughMoney();
            }
            await _rabbitmqProducer.SendTransactionRequestMessage(new TransactionRequestDTO
            {
                UserId = userId,
                AccountId = account.Id,
                Amount = request.Withdrawal.Amount,
                Type = TransactionType.Withdrawal
            });
        }
        public async Task TransferMoneyToAccount(HttpContext httpContext, TransferMoneyToAccountRequest request)
        {
            var userId = ContextDataHelper.GetUserId(httpContext);
            var account = await _dbContext.Accounts.Where(x => x.Id == request.accountId).FirstOrDefaultAsync();
            var destinationAccount = await _dbContext.Accounts.Where(x => x.Id == request.DestinationAccountId).FirstOrDefaultAsync();
            if (account == null || destinationAccount == null) {  throw new AccountNotFound(); }
            if (account.UserId != userId)
            {
                throw new UserDoesntOwnTheAccount();
            }
            if (account.Status == Models.Enum.AccountStatus.Closed || destinationAccount.Status == AccountStatus.Closed)
            {
                throw new AccountIsClosed();
            }
            if (account.Balance < request.Amount)
            {
                throw new NotEnoughMoney();
            }
            if (account.Type == AccountType.BankMasterAccount)
            {
                throw new TransferToMasterAccount();
            }
            await _rabbitmqProducer.SendTransactionRequestMessage(new TransactionRequestDTO
            {
                UserId = userId,
                AccountId = account.Id,
                DestinationAccountId = destinationAccount.Id,
                Amount = request.Amount,
                Type = TransactionType.Transfer,
            });
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

        public async Task<GetTransferMoneyRatesResponse> GetTransferMoneyRates(HttpContext httpContext, GetTransferMoneyRatesRequest request)
        {
            var account = await _dbContext.Accounts.Where(x => x.Id ==  request.AccountId).FirstOrDefaultAsync();
            var destinationAccount = await _dbContext.Accounts.Where(x => x.Id == request.DestinationAccountId).FirstOrDefaultAsync();
            if (account == null || destinationAccount == null) { throw new AccountNotFound(); }
            return new GetTransferMoneyRatesResponse
            {
                Currency = destinationAccount.Currency,
                Rate = (await _uniRateAdapter.GetExchangeRates(new GetExchangeRatesRequest
                {
                    BaseCurrency = account.Currency,
                    TargetCurrency = destinationAccount.Currency,
                })).ExchangeRate,
            };
        }
    }
}
