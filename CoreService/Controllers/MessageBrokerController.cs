using CoreService.Integrations.RabbitMQ;
using CoreService.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace CoreService.Controllers
{
    [ApiController]
    [Route("test/rabbit")]
    public class MessageBrokerController : Controller
    {
        private readonly IRabbitMQService _rabbitmqService;
        public MessageBrokerController(IRabbitMQService rabbitmqService)
        {
            _rabbitmqService = rabbitmqService;
        }

        [HttpPost]
        public IActionResult Publish([FromBody] TransactionDTO transaction)
        {
            _rabbitmqService.SendMessage(transaction);
            return Ok("Всё отправилось");
        }
    }
}
