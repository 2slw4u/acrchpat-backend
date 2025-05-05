using LoanService.Models.General;
using LoanService.Models.Monitoring;

namespace LoanService.Integrations;

public interface IRabbitMqLogOperationResultProducer
{
    Task SendOperationLogResultMessage(OperationResultLogDTO message);
}