using Microsoft.AspNetCore.Mvc;
using UserService.Services.Interfaces;

namespace UserService.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;
		public RolesController(IRolesService rolesService)
		{
            _rolesService = rolesService;
		}

		[HttpGet]
		[Route("all")]
		public async  Task<IActionResult> GetRoles()
        {
			var result = await _rolesService.GetRoles();
			return Ok(result);
		}
	}
}
