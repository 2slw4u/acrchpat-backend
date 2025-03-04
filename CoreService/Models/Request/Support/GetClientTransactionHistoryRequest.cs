using CoreService.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Request.Support
{
    public class GetClientTransactionHistoryRequest
    {
        [FromHeader]
        public Guid? user_id { get; set; }
        [Required]
        [FromRoute]
        public Guid userId { get; set; }
    }
}
