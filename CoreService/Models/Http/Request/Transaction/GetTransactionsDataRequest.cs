using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.Transaction
{
    public class GetTransactionsDataRequest
    {
        [Required]
        [FromQuery]
        public List<Guid> Transactions { get; set; }
    }
}
