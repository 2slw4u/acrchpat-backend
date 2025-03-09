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

		[HttpGet("history/{userId}")]
		[Authorize]
		[ProducesResponseType(typeof(List<BanDto>), 200)]
		public async Task<IActionResult> GetUserBanHistory(Guid userId)
		{
			var history = await _banService.GetUserBanHistory(userId);
			return Ok(history);
		}

		[HttpPost("ban/{userId}")]
		[Authorize(Roles = "Employee")]
		[ProducesResponseType(typeof(ResponseDto), 200)]
		public async Task<IActionResult> BanUser(Guid userId)
		{
			var result = await _banService.BanUser(userId);

			return Ok(result);
		}

		[HttpPost("unban/{userId}")]
		[Authorize(Roles = "Employee")]
		[ProducesResponseType(typeof(ResponseDto), 200)]
		public async Task<IActionResult> UnbanUser(Guid userId)
		{
			var result = await _banService.UnbanUser(userId);

			return Ok(result);
		}
	}
}
