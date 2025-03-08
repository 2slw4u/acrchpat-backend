using CoreService.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Cache
{
    public class UserParametersCacheEntry
    {
        public Guid Id { get; set; }
        [Required]
        public bool IsBanned { get; set; }
        public List<UserRoleDTO> Roles { get; set; }
    }
}
