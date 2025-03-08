using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace LoanService.Integrations;

public class RabbitMqService : IRabbitMqService, IDisposable
{
    private readonly ConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly string QueueName = "TestQueue";

    public RabbitMqService(IConfiguration configuration)
    {
        string backendIp = configuration["Backend:VpaIp"];
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
        
        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    public Task SendMessage(object obj)
    {
        var message = JsonSerializer.Serialize(obj);
        return SendMessage(message);
    }

    public async Task SendMessage(string message)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");
        }

        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: QueueName,
            body: body
        );
    }

    public void Dispose()
    {
        _channel?.CloseAsync();
        _connection?.CloseAsync();
    }
}