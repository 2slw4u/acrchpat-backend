using System.ComponentModel.DataAnnotations;
using LoanService.Models.General;
using LoanService.Models.Loan;
using LoanService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanService.Controllers;

[ApiController]
[Microsoft.AspNetCore.Components.Route("api/loan")]
public class LoanController(ILoanManagerService loanService) : ControllerBase
{

    /// <summary>
    /// Рассчитать кредит
    /// </summary>
    /// <response code="200">Всё ок</response>
    /// <response code="400">Invalid arguments for filtration/pagination</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("terms")]
    [Authorize]
    [ProducesResponseType(typeof(LoanPreviewDto), 200)]
    [ProducesResponseType(typeof(ResponseModel), 400)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> CalculateLoan(
        [FromQuery][Range(1, Int32.MaxValue)] float givenMoney,
        [FromQuery][Range(1, Int32.MaxValue)] int termMonths,
        [FromQuery] Guid rateId)
    {
        try
        {
            var dto = await loanService.CalculateLoan(givenMoney, termMonths, rateId);

            return Ok(dto);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseModel { Status = "Error", Message = e.Message });
        }
    }
}