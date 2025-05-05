using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTOs;
using UserService.Services.Interfaces;

namespace UserService.Controllers
{
	[ApiController]
	[Route("api/roles")]
	public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;
		public RolesController(IRolesService rolesService)
		{
            _rolesService = rolesService;
		}

		[HttpGet]
		[Route("all")]
		[ProducesResponseType(typeof(List<RoleDto>), 200)]
		public async  Task<IActionResult> GetRoles([FromHeader] Guid? TraceId)
        {
			var result = await _rolesService.GetRoles();
			return Ok(result);
		}
	}
}
