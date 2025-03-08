namespace CoreService.Integrations.AMQP.RabbitMQ.Consumer
{
    public interface IRabbitMqConsumerService
    {
        Task ReadTransactionResultMessages();
    }
}
