using System.ComponentModel.DataAnnotations;

namespace UserService.Models.DTOs
{
	public class UserDto
	{
		[Required]
		public Guid Id { get; set; }
		[Required]
		[MinLength(1)]
		public required string FullName { get; set; }
		[Required]
		[Phone]
		public required string Phone { get; set; }
		[Required]
		[EmailAddress]
		[MinLength(1)]
		public required string Email { get; set; }

		[Required]
		[MinLength(1)]
		public required ICollection<RoleDto> Roles { get; set; }

		public bool IsBanned { get; set; }
	}
}
