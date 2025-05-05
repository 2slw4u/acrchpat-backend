using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using UserService.Models.DTOs;

namespace UserService.Integrations.AMQP.RabbitMQ.Producer
{
	public class RabbitMqProducerService : IRabbitMqProducerService, IDisposable
	{
		private readonly ConnectionFactory _connectionFactory;
		private IConnection? _connection;
		private IChannel? _channel;

		public RabbitMqProducerService(IConfiguration configuration)
		{

			string backendIp = configuration["Backend:VpaIp"];
			_connectionFactory = new ConnectionFactory()
			{
				Uri = new Uri($"amqp://guest:guest@{backendIp}:5672")
			};
			InitializeRabbitMq().GetAwaiter().GetResult();
		}

		private async Task InitializeRabbitMq()
		{
			try
			{
				_connection = await _connectionFactory.CreateConnectionAsync();
				_channel = await _connection.CreateChannelAsync();
				await DeclareExchange("user.ban", ExchangeType.Fanout, durable: true, autoDelete: false);
                await DeclareExchange("log.operation.result", ExchangeType.Fanout, durable: true, autoDelete: false);
            }
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to initialize RabbitMQ: {ex}");
				throw;
			}
		}
		private async Task DeclareExchange(string exchangeName, string exchangeType, bool durable, bool autoDelete)
		{
			if (_channel == null)
			{
				throw new InvalidOperationException("Канал не инициализирован.");
			}
			await _channel.ExchangeDeclareAsync(
				exchange: exchangeName,
				type: exchangeType,
				durable: durable,
				autoDelete: autoDelete);
		}

		public async Task SendMessageToExchange<T>(T message, string exchangeName)
		{
			if (_channel == null)
			{
				throw new InvalidOperationException("Канал RabbitMQ не инициализирован.");
			}

			string messageJson = JsonSerializer.Serialize(message);
			byte[] body = Encoding.UTF8.GetBytes(messageJson);

			await _channel.BasicPublishAsync(
				exchange: exchangeName,
				routingKey: string.Empty,
				body: body);
		}

		public void Dispose()
		{
			_channel?.CloseAsync();
			_connection?.CloseAsync();
		}

	}

}
