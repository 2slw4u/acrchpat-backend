using System.Text;
using System.Text.Json;
using MonitorService.Database;
using MonitorService.Database.TableModels;
using MonitorService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MonitorService.Integrations.RabbitMq;

public class RabbitMqOperationResultConsumer(
    IConfiguration configuration,
    ILogger<RabbitMqOperationResultConsumer> logger,
    IServiceProvider serviceProvider
    ) : BackgroundService
{
    private string backendIp = configuration["Backend:VpaIp"];
    private const string OperationResultExchange = "log.operation.result";
    private IConnection? _connection;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;
    private const string QueueName = "MonitorServiceOperationResultConsumer";
    
    private async void InitializeOperationResultExchange()
    {
        await _channel.ExchangeDeclareAsync(
            exchange: OperationResultExchange,
            type: ExchangeType.Fanout,
            durable: true
        );
    }

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

            InitializeOperationResultExchange();

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
                exchange: OperationResultExchange,
                routingKey: string.Empty,
                cancellationToken: cancellationToken
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
            logger.LogInformation($"Operation info: {message}");
            var result = JsonSerializer.Deserialize<OperationResultLogDto>(message);
            
            if (result != null)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var operation = new OperationResult()
                    {
                        Id = result.Id,
                        OperationName = result.OperationName,
                        TraceId = result.TraceId,
                        OperationStart = result.OperationStart,
                        ExecutionTime = result.ExecutionTime,
                        IsSuccessful = result.IsSuccessful,
                        StatusCode = result.StatusCode,
                        ErrorMessage = result.ErrorMessage
                    };
                    
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await dbContext.OperationResults.AddAsync(operation);
                    await dbContext.SaveChangesAsync();
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