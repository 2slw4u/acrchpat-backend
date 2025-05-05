using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Database
{
    public class UserPushToken
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public required string Role { get; set; }

        public Guid? AccountId { get; set; }

        [Required, StringLength(512)] 
        public string FcmToken { get; set; } = default!;

        [Required] public DateTime CreatedAt { get; set; }
    }
}
