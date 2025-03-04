using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Request.Transaction
{
    public class GetTransactionsHistoryRequest
    {
        [FromHeader]
        [Required]
        public Guid? user_id { get; set; }
        [FromQuery]
        public List<Guid>? Accounts { get; set; }
    }
}
