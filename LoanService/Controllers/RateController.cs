using LoanService.Models.General;
using LoanService.Models.Rate;
using LoanService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LoanService.Controllers
{
    [ApiController]
    [Route("api/rate")]
    public class RateController : ControllerBase
    {
        private readonly IRateService _rateService;

        public RateController(IRateService rateService)
        {
            _rateService = rateService;
        }

        /// <summary>
        /// Создать новый тариф для кредита
        /// </summary>
        /// <response code="200">Тариф создан</response>
        /// <response code="400">Invalid arguments for filtration/pagination</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("new")]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> CreateRate(RateCreateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                var id = await _rateService.CreateRate(model);
                
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
        [HttpPost("list")]
        [ProducesResponseType(typeof(List<RateDto>), 200)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<IActionResult> RateList()
        {
            try
            {
                return Ok(await _rateService.RateList());
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseModel { Status = "Error", Message = e.Message });
            }
        }
    }
}
