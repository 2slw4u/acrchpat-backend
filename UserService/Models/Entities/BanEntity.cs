using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Entities
{
	public class BanEntity
	{
		[Required]
		public Guid Id { get; set; }

		[Required]
		public required UserEntity BannedBy { get; set; }
		[Required]
		public required UserEntity BannedUser { get; set; }
		[Required]
		public DateTime BanStart { get; set; }

		public DateTime? BanEnd {get; set;}
	}
	
}
