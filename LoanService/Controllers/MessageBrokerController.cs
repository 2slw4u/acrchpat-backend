/*using LoanService.Integrations;
using LoanService.Models.General;
using Microsoft.AspNetCore.Mvc;

namespace LoanService.Controllers;

[ApiController]
[Route("test/rabbit")]
public class MessageBrokerController(IRabbitMqService rabbitMqService) : Controller
{
    [HttpPost]
    public IActionResult Publish([FromBody] TransactionDto transaction)
    {
        rabbitMqService.SendMessage(transaction);
        return Ok("Всё отправилось");
    }
}*/