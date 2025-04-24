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
[Route("api/loan")]
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
        [FromQuery][Range(1, Int32.MaxValue)][Required] double givenMoney,
        [FromQuery][Range(1, Int32.MaxValue)][Required] int termDays,
        [FromQuery][Required] Guid rateId
        )
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
    [ProducesResponseType(typeof(LoanDetailDto), 200)]
    [ProducesResponseType(typeof(ResponseModel), 400)]
    [ProducesResponseType(typeof(ResponseModel), 401)]
    [ProducesResponseType(typeof(ResponseModel), 403)]
    [ProducesResponseType(typeof(ResponseModel), 404)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> CreateLoan(LoanCreateModel model)
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        var userRoles = (List<RoleDto>)HttpContext.Items["Roles"];
        
        return Ok(await loanService.CreateLoan(userId, model, userRoles));
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
    /// <response code="409">Конфликт данных</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost("{id}/pay")]
    [EndpointSummary("Оплатить часть кредита")]
    [Authorize]
    [RoleAuthorize("Client")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ResponseModel), 400)]
    [ProducesResponseType(typeof(ResponseModel), 401)]
    [ProducesResponseType(typeof(ResponseModel), 403)]
    [ProducesResponseType(typeof(ResponseModel), 404)]
    [ProducesResponseType(typeof(ResponseModel), 409)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> PayLoan(
        Guid id,
        [FromQuery] Guid? paymentId,
        [FromQuery] Guid? accountId
        )
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        return Ok(await loanService.PayLoan(userId, id, paymentId, accountId));
    }
    
    /// <response code="200">Кредит удален</response>
    /// <response code="401">Неавторизован</response>
    /// <response code="403">Нет полномочий</response>
    /// <response code="404">Данные не найдены</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpDelete("{id}")]
    [EndpointSummary("Удалить кредит")]
    [Authorize]
    [RoleAuthorize("Employee")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ResponseModel), 400)]
    [ProducesResponseType(typeof(ResponseModel), 401)]
    [ProducesResponseType(typeof(ResponseModel), 403)]
    [ProducesResponseType(typeof(ResponseModel), 404)]
    [ProducesResponseType(typeof(ResponseModel), 409)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> DeleteLoan([Required] Guid id)
    {
        await loanService.DeleteInvalidLoan(id);
        return Ok();
    }
    
    /// <response code="200">Ваша кредитная история получена</response>
    /// <response code="400">Неверные входные данные</response>
    /// <response code="401">Неавторизован</response>
    /// <response code="403">Нет полномочий</response>
    /// <response code="404">Данные не найдены</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("my-history")]
    [EndpointSummary("Получить свою кредитную историю")]
    [RoleAuthorize("Client")]
    [ProducesResponseType(typeof(List<LoanShortDto>), 200)]
    [ProducesResponseType(typeof(ResponseModel), 400)]
    [ProducesResponseType(typeof(ResponseModel), 401)]
    [ProducesResponseType(typeof(ResponseModel), 403)]
    [ProducesResponseType(typeof(ResponseModel), 404)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> GetMyLoanHistory()
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        return Ok(await loanService.GetLoanHistory(userId));
    }
    
    /// <response code="200">Кредитная история получена</response>
    /// <response code="400">Неверные входные данные</response>
    /// <response code="401">Неавторизован</response>
    /// <response code="403">Нет полномочий</response>
    /// <response code="404">Данные не найдены</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("history")]
    [EndpointSummary("Получить кредитную историю")]
    [Authorize]
    [RoleAuthorize("Employee")]
    [ProducesResponseType(typeof(List<LoanShortDto>), 200)]
    [ProducesResponseType(typeof(ResponseModel), 400)]
    [ProducesResponseType(typeof(ResponseModel), 401)]
    [ProducesResponseType(typeof(ResponseModel), 403)]
    [ProducesResponseType(typeof(ResponseModel), 404)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> GetLoanHistory([FromQuery][Required] Guid userId)
    {
        return Ok(await loanService.GetLoanHistory(userId));
    }
    
    /// <response code="200">Ваш кредитный рейтинг получен</response>
    /// <response code="401">Неавторизован</response>
    /// <response code="403">Нет полномочий</response>
    /// <response code="404">Данные не найдены</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("my-rating")]
    [EndpointSummary("Получить свой кредитный рейтинг")]
    [Authorize]
    [RoleAuthorize("Client")]
    [ProducesResponseType(typeof(float), 200)]
    [ProducesResponseType(typeof(ResponseModel), 401)]
    [ProducesResponseType(typeof(ResponseModel), 403)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> CalculateMyLoanRating()
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        return Ok(await loanService.CalculateLoanRating(userId));
    }
    
    /// <response code="200">Ваш кредитный рейтинг получен</response>
    /// <response code="400">Неверные входные данные</response>
    /// <response code="401">Неавторизован</response>
    /// <response code="403">Нет полномочий</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("rating")]
    [EndpointSummary("Получить кредитный рейтинг")]
    [Authorize]
    [RoleAuthorize("Employee")]
    [ProducesResponseType(typeof(float), 200)]
    [ProducesResponseType(typeof(ResponseModel), 400)]
    [ProducesResponseType(typeof(ResponseModel), 401)]
    [ProducesResponseType(typeof(ResponseModel), 403)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> CalculateLoanRating([Required] Guid userId)
    {
        return Ok(await loanService.CalculateLoanRating(userId));
    }
}