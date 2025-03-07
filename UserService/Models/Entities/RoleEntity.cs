using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Entities
{
	public class RoleEntity : IdentityRole<Guid>
	{
	}
}
