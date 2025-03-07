using System.ComponentModel.DataAnnotations;
using LoanService.Attributes;
using LoanService.Middleware;
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
    /// <response code="200">Всё ок</response>
    /// <response code="400">Неверные входные данные</response>
    /// <response code="401">Неавторизован</response>
    /// <response code="403">Нет полномочий</response>
    /// <response code="404">Данные не найдены</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("terms")]
    [EndpointSummary("Рассчитать кредит")]
    [Authorize]
    [ProducesResponseType(typeof(LoanPreviewDto), 200)]
    [ProducesResponseType(typeof(ResponseModel), 400)]
    [ProducesResponseType(typeof(ResponseModel), 401)]
    [ProducesResponseType(typeof(ResponseModel), 403)]
    [ProducesResponseType(typeof(ResponseModel), 404)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> CalculateLoan(
        [FromQuery][Range(1, Int32.MaxValue)] double givenMoney,
        [FromQuery][Range(1, Int32.MaxValue)] int termDays,
        [FromQuery] Guid rateId)
    {
        return Ok(await loanService.CalculateLoan(givenMoney, termDays, rateId));
    }
    
    /// <response code="200">Кредит создан</response>
    /// <response code="400">Неверные входные данные</response>
    /// <response code="401">Неавторизован</response>
    /// <response code="403">Нет полномочий</response>
    /// <response code="404">Данные не найдены</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost("new")]
    [EndpointSummary("Создать кредит")]
    [Authorize]
    [RoleAuthorize("Client")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(typeof(ResponseModel), 400)]
    [ProducesResponseType(typeof(ResponseModel), 401)]
    [ProducesResponseType(typeof(ResponseModel), 403)]
    [ProducesResponseType(typeof(ResponseModel), 404)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> CreateLoan(LoanCreateModel model)
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        return Ok(await loanService.CreateLoan(userId, model));
    }
    
    /// <response code="200">Данные о кредите получены</response>
    /// <response code="400">Неверные входные данные</response>
    /// <response code="401">Неавторизован</response>
    /// <response code="403">Нет полномочий</response>
    /// <response code="404">Данные не найдены</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("{id}")]
    [EndpointSummary("Получить данные о кредите")]
    [Authorize]
    [ProducesResponseType(typeof(LoanDetailDto), 200)]
    [ProducesResponseType(typeof(ResponseModel), 400)]
    [ProducesResponseType(typeof(ResponseModel), 401)]
    [ProducesResponseType(typeof(ResponseModel), 403)]
    [ProducesResponseType(typeof(ResponseModel), 404)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> GetLoan(Guid id)
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        var userRoles = (List<RoleDto>)HttpContext.Items["Roles"];
        return Ok(await loanService.GetLoan(id, userId, userRoles));
    }
    
    /// <response code="200">Часть кредита оплачена</response>
    /// <response code="400">Неверные входные данные</response>
    /// <response code="401">Неавторизован</response>
    /// <response code="403">Нет полномочий</response>
    /// <response code="404">Данные не найдены</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost("{id}/pay")]
    [EndpointSummary("Оплатить часть кредита")]
    [Authorize]
    [RoleAuthorize("Client")]
    [ProducesResponseType(typeof(int), 200)]
    [ProducesResponseType(typeof(ResponseModel), 400)]
    [ProducesResponseType(typeof(ResponseModel), 401)]
    [ProducesResponseType(typeof(ResponseModel), 403)]
    [ProducesResponseType(typeof(ResponseModel), 404)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> PayLoan(Guid id, [FromQuery] Guid? paymentId)
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        return Ok(await loanService.PayLoan(userId, id, paymentId));
    }
    
    /// <response code="200">Часть кредита оплачена</response>
    /// <response code="400">Неверные входные данные</response>
    /// <response code="401">Неавторизован</response>
    /// <response code="403">Нет полномочий</response>
    /// <response code="404">Данные не найдены</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("history")]
    [EndpointSummary("Получить кредитную историю")]
    [Authorize]
    [ProducesResponseType(typeof(List<LoanShortDto>), 200)]
    [ProducesResponseType(typeof(ResponseModel), 400)]
    [ProducesResponseType(typeof(ResponseModel), 401)]
    [ProducesResponseType(typeof(ResponseModel), 403)]
    [ProducesResponseType(typeof(ResponseModel), 404)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> GetLoanHistory()
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        return Ok(await loanService.GetLoanHistory(userId));
    }
}