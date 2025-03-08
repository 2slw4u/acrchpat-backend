using CoreService.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Response.Support
{
    public class GetClientAccountsResponse
    {
        [Required]
        public List<DetailedAccountDTO> Accounts { get; set; }
    }
}
