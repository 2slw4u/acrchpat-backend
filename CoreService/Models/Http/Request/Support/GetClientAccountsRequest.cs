using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.Support
{
    public class GetClientAccountsRequest
    {
        [Required]
        [FromQuery]
        [MinLength(1)]
        public List<Guid> Users { get; set; }
    }
}
