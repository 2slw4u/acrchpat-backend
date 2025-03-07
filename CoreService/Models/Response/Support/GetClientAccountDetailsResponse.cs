using CoreService.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Response.Support
{
    public class GetClientAccountDetailsResponse
    {
        [Required]
        public DetailedAccountDTO Account { get; set; }
    }
}
