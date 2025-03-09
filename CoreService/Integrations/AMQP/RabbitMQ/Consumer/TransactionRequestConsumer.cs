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

namespace CoreService.Integrations.AMQP.RabbitMQ.Consumer
{
    public class TransactionRequestConsumer : RabbitMqConsumer
    {
        public TransactionRequestConsumer(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<TransactionRequestConsumer> logger)
            : base(configuration: configuration,
                  serviceProvider: serviceProvider,
                  logger,
                  exchange: configuration["Integrations:AMQP:Rabbit:Exchanges:TransactionRequestExchange:Name"],
                  queue: configuration["Integrations:AMQP:Rabbit:Exchanges:TransactionRequestExchange:Queues:CoreService"])
        { }
        private async Task<List<AccountEntity>> GetUserAccounts(Guid userId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                return await dbContext.Accounts.Where(x => x.UserId == userId).ToListAsync();
            }
        }
        private async Task ValidateTransactionRequest(TransactionRequestDTO request)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                if (request.Type != TransactionType.LoanAccrual && request.Type != TransactionType.LoanPayment)
                {
                    await this.SendResult(request, "This transaction type is not supported");
                } 
                if (request.AccountId == null && request.Type == TransactionType.LoanAccrual)
                {
                    await this.SendResult(request, "При пополнении необходимо указаывать идентификатор счета");
                }
                else if (request.AccountId != null)
                {
                    var userAccounts = await GetUserAccounts(request.UserId);
                    var account = await dbContext.Accounts.Where(x => x.Id == request.AccountId).FirstOrDefaultAsync();
                    if (account == null)
                    {
                        await this.SendResult(request, "Искомого счета не существует");
                    }
                    else if (!userAccounts.Select(x => x.Id).Contains((Guid)request.AccountId))
                    {
                        await this.SendResult(request, "Пользователь не владеет нужным счетом");
                    }
                    else if (account.Status == AccountStatus.Closed)
                    {
                        await this.SendResult(request, "Искомый счет уже закрыт");
                    }
                    else if (request.Type == TransactionType.LoanPayment && account.Balance < request.Amount)
                    {
                        await this.SendResult(request, "На счете недостаточно средств для списания");
                    }
                }
                else if (request.Amount < double.Epsilon)
                {
                    await this.SendResult(request, "Значение поля Amount должно быть положительным");
                }
                await dbContext.SaveChangesAsync();
            }
        }
        private async Task ChangeBalance(TransactionRequestDTO request)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                var account = await dbContext.Accounts.Where(x => x.Id == request.AccountId).FirstOrDefaultAsync();
                if (request.Type == TransactionType.LoanAccrual)
                {
                    account.Balance += request.Amount;
                }
                else
                {
                    account.Balance -= request.Amount;
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

                var transaction = mapper.Map<TransactionEntity>(request);
                if (errorMessage != null)
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                    var account = dbContext.Accounts.Where(x => x.Id == request.AccountId).FirstOrDefault();
                    transaction.Account = account;
                    await dbContext.Transactions.AddAsync(transaction);
                    await dbContext.SaveChangesAsync();
                }

                var response = new TransactionResultDTO
                {
                    TransactionId = transaction.Id,
                    PaymentId = request.PaymentId,
                    LoanId = request.LoanId,
                    Type = request.Type == TransactionType.LoanPayment ? "LoanPayment" : "LoanAccrual",
                    Status = errorMessage == null ? true : false,
                    ErrorMessage = errorMessage
                };
                _logger.LogInformation($"Sent result: {response}");
                await rabbitProducer.SendTransactionResultMessage(response);
            }
        }

        private async Task<Guid?> DetermineSuitableAccount(TransactionRequestDTO request)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                var account = await dbContext.Accounts.
                    Where(x => x.UserId == request.UserId 
                    && x.Status == AccountStatus.Opened 
                    && (request.Type == TransactionType.LoanPayment && x.Balance > request.Amount)).FirstOrDefaultAsync();
                if (account == null)
                {
                    return null;
                }
                return account.Id;
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

                    await this.ValidateTransactionRequest(request);
                    var suitableAccountId = request.AccountId == null ? await this.DetermineSuitableAccount(request) : null;
                    if (suitableAccountId == null)
                    {
                        await this.SendResult(request, "Не нашлось подходящего счета");
                    }
                    else
                    {
                        request.AccountId = suitableAccountId;
                        await this.ChangeBalance(request);
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
