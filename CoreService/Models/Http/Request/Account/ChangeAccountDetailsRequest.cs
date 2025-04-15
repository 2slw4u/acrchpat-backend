using CoreService.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.Account
{
    public class ChangeAccountDetailsRequest
    {
        [FromRoute]
        [Required]
        public Guid accountId { get; set; }
        [FromBody]
        [Required]
        public AccountChangeModel NewAccountParameters { get; set; }
    }
}
