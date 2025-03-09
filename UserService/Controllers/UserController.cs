using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
		[ProducesResponseType(typeof(AuthenticationResponse), 200)]
		public async Task<IActionResult> Register([FromBody] UserCreateDto newUser)
		{
			var result = await _userService.Register(newUser);
			return Ok(result);
		}

		[HttpPost]
		[Authorize(Roles = "Employee")]
		[Route("create")]
		[ProducesResponseType(typeof(UserDto), 200)]
		public async Task<IActionResult> CreateUser([FromBody] UserCreateDto newUser)
		{
			var result = await _userService.CreateUser(newUser);
			return Ok(result);
		}

		[HttpPost]
		[Route("login")]
		[ProducesResponseType(typeof(AuthenticationResponse), 200)]
		public async Task<IActionResult> Login([FromBody] LoginDto newUser)
		{
			var result = await _userService.Login(newUser);
			return Ok(result);
		}

		[HttpGet]
		[Route("currentUser")]
		[Authorize]
		[ProducesResponseType(typeof(UserDto), 200)]
		public async Task<IActionResult> GetUser()
		{
			var result = await _userService.GetUser();
			return Ok(result);
		}

		[HttpGet]
		[Route("user/{userId}")]
		[Authorize]
		[ProducesResponseType(typeof(UserDto), 200)]
		public async Task<IActionResult> GetUserById(Guid userId)
		{
			var result = await _userService.GetUserById(userId);
			return Ok(result);
		}

		[HttpGet]
		[Route("all")]
		[Authorize]
		[ProducesResponseType(typeof(List<UserDto>), 200)]
		public async Task<IActionResult> GetUsers([FromQuery] Guid? roleId)
		{
			var result = await _userService.GetUsers(roleId);
			return Ok(result);
		}

		[HttpPost]
		[Route("{userId}/roles/add")]
		[Authorize(Roles = "Employee")]
		[ProducesResponseType(typeof(ResponseDto), 200)]
		public async Task<IActionResult> AddRole(Guid userId, [FromQuery][Required] Guid roleId )
		{
			var result = await _userService.AddRole(userId, roleId);
			return Ok(result);
		}

		[HttpPost]
		[Authorize(Roles = "Employee")]
		[Route("{userId}/roles/remove")]
		[ProducesResponseType(typeof(ResponseDto), 200)]
		public async Task<IActionResult> RemoveRole(Guid userId, [FromQuery][Required] Guid roleId)
		{
			var result = await _userService.RemoveRole(userId, roleId);
			return Ok(result);
		}
	}
}
