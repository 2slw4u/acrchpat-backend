using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using System.Threading;

namespace CoreService.Integrations.AMQP.RabbitMQ.Consumer
{
    public abstract class RabbitMqConsumer(IConfiguration configuration,
        IServiceProvider serviceProvider) : BackgroundService
    {
        protected IConnection? _connection;
        protected IChannel? _channel;
        private AsyncEventingBasicConsumer? _consumer;
        private string _connectionUri = configuration["Integrations:AMQP:Rabbit:Connection"];
        protected string _queue;
        protected string _exchange;

        private async Task InitializeExchange()
        {
            await _channel.ExchangeDeclareAsync(
                exchange: _exchange,
                type: ExchangeType.Fanout
            );
        }

        private async Task InitializeQueue(CancellationToken cancellationToken)
        {
            await _channel.QueueDeclareAsync(
                    queue: _queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: cancellationToken
                );

            await _channel.QueueBindAsync(
                queue: _queue,
                exchange: _exchange,
                routingKey: string.Empty,
                cancellationToken: cancellationToken
            );
        }

        private async Task InitializeRabbitMqAsync(CancellationToken cancellationToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(_connectionUri)
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync();

                await InitializeExchange();

                await InitializeQueue(cancellationToken);

                Console.WriteLine("RabbitMQ connected and queue declared.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to RabbitMQ: {ex.Message}");
                throw;
            }
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
                queue: _queue,
                autoAck: false,
                consumerTag: "",
                noLocal: false,
            exclusive: false,
                arguments: null,
                consumer: _consumer,
                cancellationToken: stoppingToken
            );

            Console.WriteLine("RabbitMQ Consumer started.");
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        protected abstract Task HandleMessageAsync(object sender, BasicDeliverEventArgs ea);

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("RabbitMQ Consumer stopping...");

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
}
