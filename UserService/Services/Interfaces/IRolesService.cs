using UserService.Models.DTOs;
using UserService.Models.Entities;

namespace UserService.Services.Interfaces
{
	public interface IRolesService
	{
		Task<List<RoleDto>> GetRoles();
		Task<RoleEntity> FindByName(string name);
	}
}
