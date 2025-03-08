using CoreService.Models.DTO;
using CoreService.Models.Exceptions;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace CoreService.Integrations.AMQP.RabbitMQ.Producer
{
    public class RabbitMqProducerService : IRabbitMqProducerService, IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private const string transactionResultExchange = "transaction.result";
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqProducerService(IConfiguration configuration)
        {
            _connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
            InitializeRabbitMq();
        }

        private async void InitializeRabbitMq()
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            this.InitializeTransactionResultExchange();
        }

        private async void InitializeTransactionResultExchange()
        {
            await _channel.ExchangeDeclareAsync(
                exchange: transactionResultExchange,
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
                throw new OperationNotImplemented();
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

        public Task SendTransactionResultMessage(TransactionResultDTO message)
        {
            return this.SendMessage(message, transactionResultExchange);
        }
    }
}
