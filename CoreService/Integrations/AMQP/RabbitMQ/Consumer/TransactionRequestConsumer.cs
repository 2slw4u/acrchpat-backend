using AutoMapper;
using CoreService.Helpers.Cache;
using CoreService.Models.Cache;
using CoreService.Models.DTO;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using CoreService.Integrations.AMQP.RabbitMQ.Producer;
using CoreService.Models.Enum;
using CoreService.Models.Database.Entity;
using CoreService.Models.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CoreService.Models.Exceptions;
using CoreService.Integrations.Http.UniRate;
using CoreService.Models.Http.Request.UniRate;

namespace CoreService.Integrations.AMQP.RabbitMQ.Consumer
{
    public class TransactionRequestConsumer : RabbitMqConsumer
    {
        private readonly IUniRateAdapter _uniRateAdapter;

        private readonly List<TransactionType> transactionTypesRequiringAccountId
            = new List<TransactionType> { TransactionType.Withdrawal, TransactionType.Deposit, TransactionType.Transfer } ;
        private readonly List<TransactionType> transactionTypesRequiringDestinationAccountId
            = new List<TransactionType> { TransactionType.Transfer };
        private readonly List<TransactionType> transactionTypesRequiringMasterAccount
            = new List<TransactionType> { TransactionType.LoanAccrual, TransactionType.LoanPayment };
        private readonly List<TransactionType> transactionTypesRequiringBalanceDeduction
            = new List<TransactionType> { TransactionType.Withdrawal, TransactionType.LoanAccrual, TransactionType.Transfer };
        public TransactionRequestConsumer(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<TransactionRequestConsumer> logger, IUniRateAdapter uniRateAdapter) 
            : base(configuration: configuration,
                  serviceProvider: serviceProvider,
                  logger,
                  exchange: configuration["Integrations:AMQP:Rabbit:Exchanges:TransactionRequestExchange:Name"],
                  queue: configuration["Integrations:AMQP:Rabbit:Exchanges:TransactionRequestExchange:Queues:CoreService"])
        {
            _uniRateAdapter = uniRateAdapter;
        }
        private async Task<AccountEntity?> GetAccountById(Guid? accountId)
        {
            if (accountId == null) return null;
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                return await dbContext.Accounts.Where(x => x.Id == accountId).FirstOrDefaultAsync();
            }
        }
        private async Task<List<AccountEntity>> GetUserAccounts(Guid userId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                return await dbContext.Accounts.Where(x => x.UserId == userId).ToListAsync();
            }
        }

        private async Task<AccountEntity?> GetMasterAccount()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                return await dbContext.Accounts.Where(x => x.Type == AccountType.BankMasterAccount).FirstOrDefaultAsync();
            }
        }
        private string? ValidateAccount(AccountEntity? account)
        {
            if (account == null)
            {
                return "Искомого счета не существует";
            }
            if (account.Status == AccountStatus.Closed)
            {
                return "Искомый счет уже закрыт";
            }
            return null;
        }
        private async Task<string?> ValidateTransactionRequest(TransactionRequestDTO request)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                /*if (request.Type != TransactionType.LoanAccrual && request.Type != TransactionType.LoanPayment)
                {
                    return "This transaction type is not supported";
                } */
                if (request.Amount < double.Epsilon)
                {
                    return "Значение поля Amount должно быть положительным";
                }
                if (request.AccountId == null && transactionTypesRequiringAccountId.Contains(request.Type))
                {
                    return "При выбранном типе транзакции необходимо указать идентификатор счета";
                }
                if (transactionTypesRequiringMasterAccount.Contains(request.Type))
                {
                    var masterAccount = await GetMasterAccount();
                    if (masterAccount == null)
                    {
                        return "Мастер-счет отсутствует в системе";
                    }
                    if (transactionTypesRequiringMasterAccount
                        .Intersect(transactionTypesRequiringBalanceDeduction)
                        .Contains(request.Type))
                    {
                        if (masterAccount.Balance <= request.Amount)
                        {
                            return "У банка кончились деньги :(";
                        }
                    }
                }
                var userAccounts = await GetUserAccounts(request.UserId);
                var account = await GetAccountById(request.AccountId);
                if (ValidateAccount(account) != null) return ValidateAccount(account);
                if (transactionTypesRequiringDestinationAccountId.Contains(request.Type))
                {
                    var destinationAccount = await GetAccountById(request.DestinationAccountId);
                    if (ValidateAccount(destinationAccount) != null) return ValidateAccount(destinationAccount);
                }
                if (!userAccounts.Select(x => x.Id).Contains(account.Id))
                {
                    return "Пользователь не владеет нужным счетом";
                }
                if (request.Type == TransactionType.LoanPayment && account.Balance < request.Amount)
                {
                    return "На счете недостаточно средств для списания";
                }
                return null;
            }
        }
        private async Task ChangeAccountBalance(AccountEntity? account, double amount, bool deposit)
        {
            if (account == null)
            {
                return;
            }
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                var accountEntity = await dbContext.Accounts.Where(x => x.Id == account.Id).FirstOrDefaultAsync();
                if (deposit)
                {
                    accountEntity.Balance += amount;
                }
                else
                {
                    accountEntity.Balance -= amount;
                }
                await dbContext.SaveChangesAsync();
            }
        }

        private async Task SendResult(TransactionRequestDTO request, string? errorMessage = null)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var rabbitProducer = scope.ServiceProvider.GetRequiredService<IRabbitMqProducer>();

                var account = await GetAccountById(request.AccountId);
                if (account == null)
                {
                    errorMessage = "Искомый счёт не найден";
                }
                AccountEntity? destinationAccount = null;
                double? destinationAmount = null;
                if (request.Type == TransactionType.Transfer)
                {
                    destinationAccount = await GetAccountById(request.DestinationAccountId);
                    var convertedAmount = destinationAccount == null ? null : await _uniRateAdapter.ConvertCurrency(new ConvertCurrencyRequest
                    {
                        BaseCurrency = account.Currency,
                        TargetCurrency = destinationAccount.Currency,
                        Amount = request.Amount,
                    });
                    destinationAmount = destinationAccount == null ? null : convertedAmount.Amount;
                }
                var transactionId = Guid.NewGuid();
                if (errorMessage == null)
                {
                    var transaction = mapper.Map<TransactionEntity>(request, opt =>
                    opt.AfterMap(async (src, dest) =>
                    {
                        dest.AccountId = account.Id;
                        dest.DestinationAccountId = request.DestinationAccountId;
                        dest.Currency = account.Currency;
                        dest.DestinationCurrency = destinationAccount == null ? null : destinationAccount.Currency;
                        dest.DestinationAmount = destinationAccount == null ? null : destinationAmount;
                    }));
                    transaction.Id = transactionId;
                    var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                    await dbContext.Transactions.AddAsync(transaction);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"New transaction in DB: {transaction.Id.ToString()}");
                }

                var response = new TransactionResultDTO
                {
                    TransactionId = transactionId,
                    PaymentId = request.PaymentId,
                    LoanId = request.LoanId,
                    Type = request.Type,
                    Status = errorMessage == null ? true : false,
                    ErrorMessage = errorMessage
                };
                _logger.LogInformation($"TransactionResultExchange message: {response.TransactionId.ToString()}");
                await rabbitProducer.SendTransactionResultMessage(response);
            }
        }

        private async Task<Guid?> DetermineSuitableAccount(TransactionRequestDTO request)
        {
            if (transactionTypesRequiringAccountId.Contains(request.Type)) return null;
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                var suitableAccounts = await dbContext.Accounts.
                    Where(x => x.UserId == request.UserId 
                    && x.Status == AccountStatus.Opened 
                    && (request.Type == TransactionType.LoanPayment && x.Balance > request.Amount)).ToListAsync();
                var account = request.Type == TransactionType.LoanPayment 
                    ? suitableAccounts.Where(x => x.Currency == CurrencyISO.RUB).FirstOrDefault() 
                    : suitableAccounts.FirstOrDefault();
                if (account == null)
                {
                    return null;
                }
                return account.Id;
            }
        }

        private async Task PerformTransaction(TransactionRequestDTO request)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                var masterAccount = await GetMasterAccount();
                var account = await GetAccountById(request.AccountId);
                var destinationAccount = await GetAccountById(request.DestinationAccountId);
                switch (request.Type)
                {
                    case TransactionType.LoanPayment:
                        await this.ChangeAccountBalance(masterAccount, request.Amount, true);
                        await this.ChangeAccountBalance(account, request.Amount, false);
                        break;
                    case TransactionType.LoanAccrual:
                        await this.ChangeAccountBalance(masterAccount, request.Amount, false);
                        await this.ChangeAccountBalance(account, request.Amount, true);
                        break;
                    case TransactionType.Deposit:
                        await this.ChangeAccountBalance(account, request.Amount, true);
                        break;
                    case TransactionType.Withdrawal:
                        await this.ChangeAccountBalance(account, request.Amount, false);
                        break;
                    case TransactionType.Transfer:
                        var destinationAccountAmount = await _uniRateAdapter.ConvertCurrency(new ConvertCurrencyRequest
                        {
                            BaseCurrency = account.Currency,
                            TargetCurrency = destinationAccount.Currency,
                            Amount = request.Amount,
                        });
                        await this.ChangeAccountBalance(account, request.Amount, false);
                        await this.ChangeAccountBalance(destinationAccount, destinationAccountAmount.Amount, true);
                        break;
                }
            }
        }

        protected override async Task HandleMessageAsync(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var request = JsonSerializer.Deserialize<TransactionRequestDTO>(message);

                if (request != null)
                {
                    _logger.LogInformation(message);

                    var suitableAccountId = request.AccountId == null ? await this.DetermineSuitableAccount(request) : request.AccountId;
                    if (suitableAccountId == null)
                    {
                        await this.SendResult(request, "Не нашлось подходящего счета");
                    }
                    request.AccountId = suitableAccountId;
                    var errorMessage = await this.ValidateTransactionRequest(request);
                    if (errorMessage != null)
                    {
                        await this.SendResult(request, errorMessage);
                    }
                    else
                    {
                        await this.PerformTransaction(request);
                        await this.SendResult(request);
                    }
                }
                if (_channel != null)
                {
                    await _channel.BasicAckAsync(ea.DeliveryTag, false, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
            }
        }
    }
}
