using System.ComponentModel.DataAnnotations;

namespace UserService.Models.DTOs
{
	public class RoleDto
	{
		[Required]
		public Guid Id { get; set; }
		[Required]
		[MinLength(1)]
		public required string Name { get; set; }
	}
}
