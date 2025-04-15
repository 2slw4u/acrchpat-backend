using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.DTO
{
    public class UserDTO
    {
        [Required]
        public string Phone { get; set; }
        public Guid Id { get; set; }
        public bool IsBanned { get; set; }
        public List<UserRoleDTO> Roles { get; set; }
    }
}
