using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.Account
{
    public class CloseAccountRequest
    {
        [FromRoute]
        [Required]
        public Guid accountId { get; set; }
        [FromHeader]
        public Guid? TraceId { get; set; }
    }
}
