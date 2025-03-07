using Microsoft.EntityFrameworkCore;
using UserService.Database;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Services.Interfaces;

namespace UserService.Services
{
	public class RolesService : IRolesService
	{

		private readonly AppDbContext _context;
		private readonly ILogger<RolesService> _logger;

		public RolesService(AppDbContext context, ILogger<RolesService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<List<RoleDto>> GetRoles()
		{
			var roles = await _context.Roles.Select(r => new RoleDto { Id = r.Id, Name = r.Name}).ToListAsync();
			return roles;
		}

		public async Task<RoleEntity> FindByName(string name)
		{
			var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
			return role;
		}
	}
}
