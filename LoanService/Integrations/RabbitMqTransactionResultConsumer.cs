using System.Text;
using System.Text.Json;
using LoanService.Models.General;
using LoanService.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LoanService.Integrations;

public class RabbitMqTransactionResultConsumer(
    IConfiguration configuration,
    ILogger<RabbitMqTransactionResultConsumer> logger,
    ILoanManagerService loanService
    ) : BackgroundService
{
    private string backendIp = configuration["Backend:VpaIp"];
    private const string TransactionResultExchange = "transaction.result";
    private IConnection? _connection;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;
    private const string QueueName = "LoanConsumer";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeRabbitMqAsync(stoppingToken);
        
        if (_channel == null)
        {
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");
        }
        
        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.ReceivedAsync += HandleMessageAsync;

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumerTag: "",
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: _consumer,
            cancellationToken: stoppingToken
        );

        logger.LogInformation("RabbitMQ Consumer started.");
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
    
    private async Task InitializeRabbitMqAsync(CancellationToken cancellationToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri($"amqp://guest:guest@{backendIp}:5672")
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            await _channel.QueueBindAsync(
                queue: QueueName,
                exchange: TransactionResultExchange,
                routingKey: string.Empty
            );

            logger.LogInformation("RabbitMQ connected and queue declared.");
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to connect to RabbitMQ: {ex.Message}");
            throw;
        }
    }
    
    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var result = JsonSerializer.Deserialize<TransactionResult>(message);

            if (result != null)
            {
                logger.LogInformation($"Received transaction request result for loan with ID {result.LoanId}," +
                                      $" it is a {result.Status}. Transaction ID: {result.TransactionId}, payment ID:" +
                                      $" {result.PaymentId} error message: {result.ErrorMessage}");
                
                if (result.Status == TransactionResultStatus.Success)
                {
                    await loanService.AddTransaction(result.LoanId, result.TransactionId, result.PaymentId);
                }
                else
                {
                    if (result.Type == TransactionType.LoanPayment)
                    {
                        await loanService.MarkPaymentAsOverdue(result.LoanId, result.PaymentId);
                    }
                    else if (result.Type == TransactionType.LoanAccrual)
                    {
                        await loanService.DeleteInvalidLoan(result.LoanId);
                    }
                }
            }
            
            if (_channel != null)
            {
                await _channel.BasicAckAsync(ea.DeliveryTag, false, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Error processing message: {ex.Message}");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("RabbitMQ Consumer stopping...");

        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
        }
    }

    public override void Dispose()
    {
        _channel?.CloseAsync();
        _connection?.CloseAsync();
        base.Dispose();
    }
}