using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PreferenceService.Models;
using PreferenceService.Services;

namespace PreferenceService.Controller;

[ApiController]
[Route("api")]
public class PreferenceController(IPreferenceManager preferenceManager) : ControllerBase
{
    /// <response code="200">Тема получена</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("theme")]
    [EndpointSummary("Получить тему")]
    [Authorize]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> GetTheme()
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        return Ok(await preferenceManager.GetTheme(userId));
    }
    
    /// <response code="200">Тема изменена</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost("theme")]
    [EndpointSummary("Изменить тему")]
    [Authorize]
    [ProducesResponseType( 200)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> SetTheme([Required] ThemeType theme)
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        await preferenceManager.SetTheme(userId, theme);
        return Ok();
    }
    
    /// <response code="200">Список получен</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("hidden-accounts")]
    [EndpointSummary("Получить список скрытых счетов")]
    [Authorize]
    [ProducesResponseType(typeof(List<Guid>), 200)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> GetHiddenAccountList()
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        return Ok(await preferenceManager.GetHiddenAccountList(userId));
    }
    
    /// <response code="200">Счета скрыты</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost("hidden-accounts/hide")]
    [EndpointSummary("Скрыть счета")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> HideAccounts([Required] List<Guid> accounts)
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        await preferenceManager.HideAccounts(userId, accounts);
        return Ok();
    }
    
    /// <response code="200">Счета раскрыты</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost("hidden-accounts/unhide")]
    [EndpointSummary("Раскрыть счета")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ResponseModel), 500)]
    public async Task<IActionResult> UnhideAccounts([Required] List<Guid> accounts)
    {
        var userId = (Guid)HttpContext.Items["UserId"];
        await preferenceManager.UnhideAccounts(userId, accounts);
        return Ok();
    }
}