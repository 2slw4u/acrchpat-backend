using System.ComponentModel.DataAnnotations;

namespace UserService.Models.DTOs
{
	public class LoginDto
	{
		[Required]
		[Phone]
		public required string Phone { get; set; }
		[Required]
		[MinLength(6)]
		public required string Password { get; set; }
	}
}
