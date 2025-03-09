
using CoreService.Models.DTO;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CoreService.Integrations.AMQP.RabbitMQ.Producer
{
    public class RabbitMqProducer : IRabbitMqProducer, IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection? _connection;
        private IChannel? _channel;
        private Dictionary<string, string> _exchanges;
        public RabbitMqProducer(IConfiguration configuration)
        {
            _exchanges = new Dictionary<string, string>();
            _connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri($"{configuration["Integrations:AMQP:Rabbit:Connection"]}")
            };
            this.InitializeRabbitMq(configuration);
        }
        private async Task InitializeRabbitMq(IConfiguration configuration)
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await this.InitializeExchanges(configuration);
        }
        private async Task InitializeExchanges(IConfiguration configuration)
        {
            await this.InitializeTransactionResultExchage(configuration);
        }
        private async Task InitializeTransactionResultExchage(IConfiguration configuration)
        {
            var transactionResultExchange = $"{configuration["Integrations:AMQP:Rabbit:Exchange:TransactionResultExchange:Name"]}";
            await _channel.ExchangeDeclareAsync(
                exchange: transactionResultExchange,
                type: ExchangeType.Fanout,
                durable: true
            );
            _exchanges.Add("TransactionResult", transactionResultExchange);
        }
        private async Task SendMessage(object obj, string exchange)
        {
            var message = JsonSerializer.Serialize(obj);
            await SendMessage(message, exchange);
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

        public async Task SendTransactionResultMessage(TransactionResultDTO message)
        {
            var transactionResultExchange = _exchanges.FirstOrDefault(x => x.Key == "TransactionResult").Value;
            await this.SendMessage(message, transactionResultExchange);
        }
    }
}
