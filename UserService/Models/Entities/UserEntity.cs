using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;

namespace UserService.Models.Entities
{
	public class UserEntity : IdentityUser<Guid>
	{
        [Required]
		[MinLength(1)]
		public required string FullName { get; set; }

		public ICollection<BanEntity> Bans { get; set; } = new List<BanEntity>();

		[Required]
		[MinLength(1)]
		public required ICollection<RoleEntity> Roles { get; set; }
	}
}
