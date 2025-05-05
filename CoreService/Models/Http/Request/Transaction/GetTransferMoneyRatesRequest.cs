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
        public string DestinationAccountNumber { get; set; }
        [FromHeader]
        public Guid? TraceId { get; set; }
    }
}
