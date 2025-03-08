using CoreService.Models.DTO;

namespace CoreService.Integrations.AMQP.RabbitMQ.Producer;

public interface IRabbitMqProducerService
{
    Task SendTransactionResultMessage(TransactionResultDTO message);
}