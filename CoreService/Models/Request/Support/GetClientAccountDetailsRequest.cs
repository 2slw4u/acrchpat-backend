using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Request.Support
{
    public class GetClientAccountDetailsRequest
    {
        [FromHeader]
        public Guid? user_id { get; set; }
        [Required]
        [FromRoute]
        public Guid accountId { get; set; }
    }
}
