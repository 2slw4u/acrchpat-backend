namespace UserService.Integrations.AMQP.RabbitMQ.Producer
{
	public interface IRabbitMqProducerService
	{
		Task SendMessageToExchange<T>(T message, string exchangeName);
	}

}
