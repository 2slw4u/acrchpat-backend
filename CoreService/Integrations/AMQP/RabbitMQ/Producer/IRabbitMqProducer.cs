using CoreService.Models.DTO;

namespace CoreService.Integrations.AMQP.RabbitMQ.Producer
{
    public interface IRabbitMqProducer
    {
        Task SendTransactionResultMessage(TransactionResultDTO message);
    }
}
