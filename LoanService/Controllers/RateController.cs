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

        /// <summary>
        /// Создать новый тариф для кредита
        /// </summary>
        /// <response code="200">Тариф создан</response>
        /// <response code="400">Invalid arguments for filtration/pagination</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("new")]
        [Authorize]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> CreateRate(RateCreateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                var id = await rateService.CreateRate(model);
                
                return Ok(id);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseModel { Status = "Error", Message = e.Message });
            }
        }

        /// <summary>
        /// Список доступных тарифов для кредита
        /// </summary>
        /// <response code="200">Список получен</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("list")]
        [Authorize]
        [ProducesResponseType(typeof(List<RateDto>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> RateList()
        {
            try
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseModel { Status = "Error", Message = HttpContext.Items["UserId"]?.ToString() });
                return Ok(await rateService.RateList());
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseModel { Status = "Error", Message = e.Message });
            }
        }
    }
}
