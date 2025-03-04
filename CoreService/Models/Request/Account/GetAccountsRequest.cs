using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Request.Account
{
    public class GetAccountsRequest
    {
        [FromHeader]
        [Required]
        public Guid user_id { get; set; }
    }
}
