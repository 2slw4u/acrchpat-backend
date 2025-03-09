using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTOs;
using UserService.Services.Interfaces;

namespace UserService.Controllers
{
	[ApiController]
	[Route("api/ban")]
	[Authorize]
	public class BanController : ControllerBase
	{
		private readonly IBanService _banService;

		public BanController(IBanService banService)
		{
			_banService = banService;
		}

		/// <summary>
		/// Получить историю банов для указанного пользователя.
		/// </summary>
		/// <param name="userId">Идентификатор пользователя</param>
		/// <returns>Список банов</returns>
		[HttpGet("history/{userId}")]
		[Authorize]
		[ProducesResponseType(typeof(List<BanDto>), 200)]
		public async Task<IActionResult> GetUserBanHistory(Guid userId)
		{
			var history = await _banService.GetUserBanHistory(userId);
			return Ok(history);
		}

		/// <summary>
		/// Забанить пользователя. Банить могут только сотрудники (employee).
		/// </summary>
		/// <param name="userId">Идентификатор пользователя для бана</param>
		/// <returns>Результат операции</returns>
		[HttpPost("ban/{userId}")]
		[Authorize]
		[ProducesResponseType(typeof(ResponseDto), 200)]
		public async Task<IActionResult> BanUser(Guid userId)
		{
			var result = await _banService.BanUser(userId);

			return Ok(result);
		}

		/// <summary>
		/// Разбанить пользователя. Разбанивать могут только сотрудники (employee).
		/// </summary>
		/// <param name="userId">Идентификатор пользователя для разбана</param>
		/// <returns>Результат операции</returns>
		[HttpPost("unban/{userId}")]
		[Authorize]
		[ProducesResponseType(typeof(ResponseDto), 200)]
		public async Task<IActionResult> UnbanUser(Guid userId)
		{
			var result = await _banService.UnbanUser(userId);

			return Ok(result);
		}
	}
}
