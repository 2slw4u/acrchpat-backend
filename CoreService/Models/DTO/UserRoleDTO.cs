using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.DTO
{
    public class UserRoleDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
