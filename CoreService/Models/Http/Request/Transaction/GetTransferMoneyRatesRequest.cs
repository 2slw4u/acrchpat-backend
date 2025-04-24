using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.Transaction
{
    public class GetTransferMoneyRatesRequest
    {
        [Required]
        [FromQuery]
        public Guid AccountId { get; set; }
        [Required]
        [FromQuery]
        public Guid DestinationAccountId { get; set; }
    }
}
