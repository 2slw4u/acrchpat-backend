using System.ComponentModel.DataAnnotations;
using UserService.Models.Entities;

namespace UserService.Models.DTOs
{
	public class BanDto
	{
		[Required]
		public Guid Id { get; set; }

		[Required]
		public required Guid BannedBy { get; set; }
		[Required]
		public required Guid BannedUser { get; set; }
		[Required]
		public DateTime BanStart { get; set; }

		public DateTime? BanEnd { get; set; }
	}
}
