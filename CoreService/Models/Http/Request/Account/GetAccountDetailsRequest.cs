using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.Account
{
    public class GetAccountDetailsRequest
    {
        [FromHeader]
        public Guid? user_id { get; set; }
        [FromRoute]
        [Required]
        public Guid accountId { get; set; }
        [FromHeader]
        public Guid? TraceId { get; set; }
    }
}
