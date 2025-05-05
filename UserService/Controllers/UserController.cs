using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserService.Models.DTOs;
using UserService.Services.Interfaces;


namespace UserService.Controllers
{
	[ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
		[AllowAnonymous]
		[ProducesResponseType(typeof(AuthenticationResponse), 200)]
		public async Task<IActionResult> Register([FromBody] UserCreateDto newUser, [FromHeader] Guid? TraceId)
		{
			var result = await _userService.Register(newUser);
			return Ok(result);
		}

		[HttpPost]
		[Authorize(Roles = "Employee")]
		[Route("create")]
		[ProducesResponseType(typeof(UserDto), 200)]
		public async Task<IActionResult> CreateUser([FromBody] UserCreateDto newUser, [FromHeader] Guid? TraceId)
		{
			var result = await _userService.CreateUser(newUser);
			return Ok(result);
		}

		[HttpPost]
		[Route("login")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(AuthenticationResponse), 200)]
		public async Task<IActionResult> Login([FromBody] LoginDto newUser, [FromHeader] Guid? TraceId)
		{
			var result = await _userService.Login(newUser);
			return Ok(result);
		}

		[HttpGet]
		[Route("currentUser")]
		[ProducesResponseType(typeof(UserDto), 200)]
		public async Task<IActionResult> GetUser([FromHeader] Guid? TraceId)
		{
			var result = await _userService.GetUser();
			return Ok(result);
		}

		[HttpGet]
		[Route("user/{userId}")]
		[ProducesResponseType(typeof(UserDto), 200)]
		public async Task<IActionResult> GetUserById(Guid userId, [FromHeader] Guid? TraceId)
		{
			var result = await _userService.GetUserById(userId);
			return Ok(result);
		}

		[HttpGet]
		[Route("all")]
		[ProducesResponseType(typeof(List<UserDto>), 200)]
		public async Task<IActionResult> GetUsers([FromQuery] Guid? roleId, [FromHeader] Guid? TraceId)
		{
			var result = await _userService.GetUsers(roleId);
			return Ok(result);
		}

		[HttpPost]
		[Route("{userId}/roles/add")]
		[Authorize(Roles = "Employee")]
		[ProducesResponseType(typeof(ResponseDto), 200)]
		public async Task<IActionResult> AddRole(Guid userId, [FromQuery][Required] Guid roleId, [FromHeader] Guid? TraceId)
		{
			var result = await _userService.AddRole(userId, roleId);
			return Ok(result);
		}

		[HttpPost]
		[Authorize(Roles = "Employee")]
		[Route("{userId}/roles/remove")]
		[ProducesResponseType(typeof(ResponseDto), 200)]
		public async Task<IActionResult> RemoveRole(Guid userId, [FromQuery][Required] Guid roleId, [FromHeader] Guid? TraceId)
		{
			var result = await _userService.RemoveRole(userId, roleId);
			return Ok(result);
		}
	}
}
