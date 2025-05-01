using LoanService.Attributes;
using LoanService.Models.General;
using LoanService.Models.Rate;
using LoanService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanService.Controllers
{
    [ApiController]
    [Route("api/rate")]
    public class RateController(IRateService rateService) : ControllerBase
    {
        
        /// <response code="200">Тариф создан</response>
        /// <response code="400">Неверные входные данные</response>
        /// <response code="401">Неавторизован</response>
        /// <response code="403">Нет полномочий</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("new")]
        [EndpointSummary("Создать новый тариф для кредита")]
        [Authorize]
        [RoleAuthorize("Employee")]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 401)]
        [ProducesResponseType(typeof(ResponseModel), 403)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> CreateRate(RateCreateModel model, [FromHeader(Name = "Idempotency-Key")] string? IdempotencyKey)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            return Ok(await rateService.CreateRate(model, IdempotencyKey));
        }
        
        /// <response code="200">Список получен</response>
        /// <response code="401">Неавторизован</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("list")]
        [EndpointSummary("Список доступных тарифов для кредита")]
        [Authorize]
        [ProducesResponseType(typeof(List<RateDto>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 401)]
        [ProducesResponseType(typeof(ResponseModel), 403)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> RateList()
        {
            return Ok(await rateService.RateList());
        }
    }
}
