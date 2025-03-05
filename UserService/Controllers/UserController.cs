using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTOs;
using UserService.Services.Interfaces;

namespace UserService.Controllers
{
	[ApiController]
	[Route("api/user")]
	public class UserController : ControllerBase
    {
		private readonly IUserManagingService _userService;
		public UserController(IUserManagingService userService)
		{
			_userService = userService;
		}

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] UserCreateDto newUser)
		{
			var result = await _userService.Register(newUser);
			return Ok(result);
		}


		[HttpPost]
		[Authorize]
		[Route("create")]
		public async Task<IActionResult> CreateUser([FromBody] UserCreateDto newUser)
		{
			var result = await _userService.CreateUser(newUser);
			return Ok(result);
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto newUser)
		{
			var result = await _userService.Login(newUser);
			return Ok(result);
		}

		[HttpGet]
		[Route("currentUser")]
		[Authorize]
		public async Task<IActionResult> GetUser()
		{
			var result = await _userService.GetUser();
			return Ok(result);
		}

		[HttpGet]
		[Route("all")]
		[Authorize]
		public async Task<IActionResult> GetUsers([FromQuery] Guid? role)
		{
			var result = await _userService.GetUsers(role);
			return Ok(result);
		}

	}
}
