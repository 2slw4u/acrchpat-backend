using System.ComponentModel.DataAnnotations;
using CoreService.Models.DTO;

namespace CoreService.Models.Response.Account
{
    public class GetAccountsResponse
    {
        [Required]
        public List<AccountDTO> Accounts { get; set; }
    }
}
