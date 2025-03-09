using LoanService.Models.General;

namespace LoanService.Integrations;

public interface IRabbitMqTransactionRequestProducer
{
    Task SendTransactionRequestMessage(TransactionRequest message);
}