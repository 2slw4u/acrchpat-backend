using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CoreService.Integrations.AMQP.RabbitMQ.Consumer
{
    public class RabbitMqConsumerService : IRabbitMqConsumerService, IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private const string transactionResultExchange = "transaction.result";
        private const string queueName = "TransactionResultInnerQueue";
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqConsumerService(IConfiguration configuration)
        {
            _connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
            InitializeRabbitMq();
        }

        private async Task InitializeRabbitMq()
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await this.InitializeTransactionResultInnerQueue();
        }

        private async Task InitializeTransactionResultInnerQueue()
        {
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false);
            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: transactionResultExchange,
                routingKey: string.Empty);
        }

        public async Task ReadTransactionResultMessages()
        {
            var consumer = new AsyncEventingBasicConsumer(channel: _channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"!!!!!!!!!!!!!!!!consumed message: {message}");
                return Task.CompletedTask;
            };
            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer);
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
        }
    }
}
