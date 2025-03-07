using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace RabbitConsumer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RabbitConsumer : ControllerBase
    {
       
        private readonly IMemoryCache _mMemoryCache;

        public RabbitConsumer(IMemoryCache memoryCache)
        {
            _mMemoryCache = memoryCache;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public Transaction Get(Guid transactionId)
        {
            throw new NotImplementedException();
        }
    }
}
