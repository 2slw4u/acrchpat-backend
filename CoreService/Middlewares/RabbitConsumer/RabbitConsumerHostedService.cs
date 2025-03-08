
using CoreService.Integrations.AMQP.RabbitMQ.Consumer;

namespace CoreService.Middlewares.RabbitConsumer
{
    public class RabbitConsumerHostedService : BackgroundService
    {
        private readonly IRabbitMqConsumerService _rabbitMqConsumerService;
        public RabbitConsumerHostedService(IRabbitMqConsumerService consumerService)
        {
            _rabbitMqConsumerService = consumerService;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _rabbitMqConsumerService.ReadTransactionResultMessages();
        }
    }
}
