using System.Text;
using System.Text.Json;
using LoanService.Models.General;
using RabbitMQ.Client;

namespace LoanService.Integrations;

public class RabbitMqTransactionRequestProducer : IRabbitMqTransactionRequestProducer, IDisposable
{
    private readonly ConnectionFactory _connectionFactory;
    private const string TransactionRequestExchange = "transaction.request";
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly string QueueName = "CoreConsumer";

    public RabbitMqTransactionRequestProducer(IConfiguration configuration)
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
        
        // await _channel.QueueDeclareAsync(
        //     queue: QueueName,
        //     durable: false,
        //     exclusive: false,
        //     autoDelete: false,
        //     arguments: null
        // );
    }
    
    private async void InitializeTransactionRequestExchange()
    {
        await _channel.ExchangeDeclareAsync(
            exchange: TransactionRequestExchange,
            type: ExchangeType.Fanout
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
    
    public Task SendTransactionRequestMessage(TransactionRequest message)
    {
        return SendMessage(message, TransactionRequestExchange);
    }
}