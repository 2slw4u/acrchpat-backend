using CoreService.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.Transaction
{
    public class DepositMoneyToAccountRequest
    {
        [FromRoute]
        [Required]
        public Guid accountId { get; set; }
        [FromBody]
        [Required]
        public MoneyChangeModel Deposit { get; set; }
        [FromHeader]
        public Guid? TraceId { get; set; }
    }
}
