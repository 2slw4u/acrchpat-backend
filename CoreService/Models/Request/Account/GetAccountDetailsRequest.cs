using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Request.Account
{
    public class GetAccountDetailsRequest
    {
        [FromHeader]
        public Guid? user_id { get; set; }
        [FromRoute]
        [Required]
        public Guid accountId { get; set; }
    }
}
