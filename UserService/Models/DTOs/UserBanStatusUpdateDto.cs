using System.ComponentModel.DataAnnotations;
using UserService.Models.Entities;

namespace UserService.Models.DTOs
{
	public class UserBanStatusUpdateDto
	{
		public UserBanStatusUpdateDto() { }
		public UserBanStatusUpdateDto(UserEntity u)
		{
			Id = u.Id;
			Phone = u.PhoneNumber;
			IsBanned = u.Bans.Any(b => b.BanEnd == null);
			Roles = u.Roles.Select(r => new RoleDto { Id = r.Id, Name = r.Name }).ToList();
		}
		[Required]
		public Guid Id { get; set; }
		[Phone]
		[Required]
		public string Phone { get; set; }

		[Required]
		[MinLength(1)]
		public ICollection<RoleDto> Roles { get; set; }

		[Required]
		public bool IsBanned { get; set; }
	}
}
