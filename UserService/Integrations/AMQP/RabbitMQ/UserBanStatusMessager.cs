using UserService.Integrations.AMQP.RabbitMQ.Producer;
using UserService.Models.DTOs;

namespace UserService.Integrations.AMQP.RabbitMQ
{
	public class UserBanStatusMessager
	{
		private readonly IRabbitMqProducerService _rabbitMqProducerService;

		public UserBanStatusMessager(IRabbitMqProducerService rabbitMqProducerService)
		{
			_rabbitMqProducerService = rabbitMqProducerService;
		}

		public async Task SendMessage(UserBanStatusUpdateDto dto)
		{
			await _rabbitMqProducerService.SendMessageToExchange(dto, "user.ban");
		}
	}
}
