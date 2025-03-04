using CoreService.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Request.Support
{
    public class ChangeClientAccountStatusRequest
    {
        [FromHeader]
        public Guid? user_id { get; set; }
        [Required]
        [FromRoute]
        public Guid accountId { get; set; }
        [Required]
        [FromBody]
        public AccountStatus NewStatus { get; set; }
    }
}
