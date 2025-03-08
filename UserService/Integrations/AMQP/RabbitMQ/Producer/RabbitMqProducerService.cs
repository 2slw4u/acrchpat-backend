using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using UserService.Models.DTOs;

namespace UserService.Integrations.AMQP.RabbitMQ.Producer
{
	public class RabbitMqProducerService : IRabbitMqProducerService, IDisposable
	{
		private readonly ConnectionFactory _connectionFactory;
		private const string userBanStatusUpdateExchange = "user.ban";
		private IConnection? _connection;
		private IChannel? _channel;
		private readonly string _queueName = "Test";

		public RabbitMqProducerService(IConfiguration configuration)
		{

			string backendIp = configuration["Backend:VpaIp"];
			_connectionFactory = new ConnectionFactory()
			{
				Uri = new Uri($"amqp://guest:guest@{backendIp}:5672")
			};
			InitializeRabbitMq();
		}

		private async Task InitializeRabbitMq()
		{
			try
			{
				_connection = await _connectionFactory.CreateConnectionAsync();
				_channel = await _connection.CreateChannelAsync();
				await InitializeTransactionResultExchange();
			}
			catch (Exception ex)
			{
				// Log or re-throw
				Console.WriteLine($"Failed to initialize RabbitMQ: {ex}");
				throw;
			}
		}

		private async Task InitializeTransactionResultExchange()
		{
			await _channel.ExchangeDeclareAsync(
				exchange: userBanStatusUpdateExchange,
				type: ExchangeType.Fanout,
				durable: true,
				autoDelete: false
			);
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
				throw new InvalidOperationException("RabbitMQ channel not initialized.");
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

		public Task SendUserBanStatusUpdateMessage(UserBanStatusUpdateDto message)
		{
			return SendMessage(message, userBanStatusUpdateExchange);
		}

	}

}
