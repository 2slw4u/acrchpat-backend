using CoreService.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Response.Account
{
    public class OpenNewAccountResponse
    {
        [Required]
        public AccountDTO NewAccount { get; set; }
    }
}
