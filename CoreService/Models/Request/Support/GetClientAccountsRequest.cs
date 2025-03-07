using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Request.Support
{
    public class GetClientAccountsRequest
    {
        [FromHeader]
        public Guid? user_id { get; set; }
        [Required]
        [FromQuery]
        public List<Guid> Users { get; set; }
    }
}
