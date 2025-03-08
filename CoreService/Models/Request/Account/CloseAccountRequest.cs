using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Request.Account
{
    public class CloseAccountRequest
    {
        [FromRoute]
        [Required]
        public Guid accountId { get; set; }
    }
}
