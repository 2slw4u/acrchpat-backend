using CoreService.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.Support
{
    public class GetAccountTransactionHistoryRequest
    {
        [Required]
        [FromRoute]
        public Guid accountId { get; set; }
        [FromHeader]
        public Guid? TraceId { get; set; }
    }
}
