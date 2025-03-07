using System.ComponentModel.DataAnnotations;
using UserService.Models.Entities;

namespace UserService.Models.DTOs
{
	public class UserCreateDto
	{
		[Required]
		[MinLength(1)]
		public required string FullName { get; set; }
		[Required]
		[MinLength(6)]
		public required string Password { get; set; }
		[Required]
		[Phone]
		public required string PhoneNumber { get; set; }
		[Required]
		[EmailAddress]
		[MinLength(1)]
		public required string Email { get; set; }

		[Required]
		[MinLength(1)]
		public required ICollection<Guid> Roles { get; set; }
	}
}
