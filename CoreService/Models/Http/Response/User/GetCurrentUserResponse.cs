using CoreService.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Response.User
{
    public class GetCurrentUserResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<UserRoleDTO> Roles { get; set; }
        [Required]
        public bool IsBanned { get; set; }
    }
}
