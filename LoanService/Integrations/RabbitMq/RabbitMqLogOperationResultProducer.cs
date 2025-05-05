using System.Text;
using System.Text.Json;
using LoanService.Models.General;
using LoanService.Models.Monitoring;
using RabbitMQ.Client;

namespace LoanService.Integrations;

public class RabbitMqLogOperationResultProducer : IRabbitMqLogOperationResultProducer, IDisposable
{
    private readonly ConnectionFactory _connectionFactory;
    private const string LogOperationResultExchange = "log.operation.result";
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqLogOperationResultProducer(IConfiguration configuration)
    {
        var backendIp = configuration["Backend:VpaIp"];
        _connectionFactory = new ConnectionFactory()
        {
            Uri = new Uri($"amqp://guest:guest@{backendIp}:5672")
        };
        InitializeRabbitMq();
    }

    private async void InitializeRabbitMq()
    {
        _connection = await _connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        InitializeTransactionRequestExchange();
    }
    
    private async void InitializeTransactionRequestExchange()
    {
        await _channel.ExchangeDeclareAsync(
            exchange: LogOperationResultExchange,
            type: ExchangeType.Fanout,
            durable: true
        );
    }

    private Task SendMessage(object obj, string exchange)
    {
        var message = JsonSerializer.Serialize(obj);
        return SendMessage(message, exchange);
    }

    private async Task SendMessage(string message, string exchange)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");
        }

        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: string.Empty,
            body: body
        );
    }

    public void Dispose()
    {
        _channel?.CloseAsync();
        _connection?.CloseAsync();
    }
    
    public Task SendOperationLogResultMessage(OperationResultLogDTO message)
    {
        return SendMessage(message, LogOperationResultExchange);
    }
}