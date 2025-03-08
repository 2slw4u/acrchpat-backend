using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Request.Support
{
    public class GetClientAccountDetailsRequest
    {
        [Required]
        [FromRoute]
        public Guid accountId { get; set; }
    }
}
