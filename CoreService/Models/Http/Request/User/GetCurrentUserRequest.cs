using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.User
{
    public class GetCurrentUserRequest
    {
        [Required]
        public string BearerToken { get; set; }
    }
}
