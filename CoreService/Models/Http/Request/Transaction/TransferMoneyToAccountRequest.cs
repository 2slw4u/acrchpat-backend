using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.Transaction
{
    public class TransferMoneyToAccountRequest
    {
        [Required]
        [FromRoute]
        public Guid accountId { get; set; }
        [Required]
        [FromQuery]
        public string DestinationAccountNumber { get; set; }
        [Required]
        [FromQuery]
        public double Amount { get; set; }
    }
}
