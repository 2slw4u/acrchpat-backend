using LoanService.Integrations;
using LoanService.Models.General;
using Microsoft.AspNetCore.Mvc;

namespace LoanService.Controllers;

[ApiController]
[Route("test/rabbit")]
public class MessageBrokerController(IRabbitMqTransactionRequestProducer rabbitMqTransactionRequestProducer) : Controller
{
    [HttpPost]
    public IActionResult Publish([FromBody] TransactionRequest transaction)
    {
        rabbitMqTransactionRequestProducer.SendTransactionRequestMessage(transaction);
        return Ok("Всё отправилось");
    }
}