using System.ComponentModel.DataAnnotations;
using CoreService.Models.DTO;

namespace CoreService.Models.Response.Account
{
    public class GetAccountDetailsResponse
    {
        [Required]
        public AccountDTO Account { get; set; }
    }
}
