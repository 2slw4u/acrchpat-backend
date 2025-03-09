using UserService.Models.DTOs;

namespace UserService.Integrations.AMQP.RabbitMQ.Producer
{
	public interface IRabbitMqProducerService
	{
		Task SendUserBanStatusUpdateMessage(UserBanStatusUpdateDto message);
	}

}
