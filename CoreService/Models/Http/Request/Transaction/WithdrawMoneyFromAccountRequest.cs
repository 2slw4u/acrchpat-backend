using CoreService.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Request.Transaction
{
    public class WithdrawMoneyFromAccountRequest
    {
        [FromRoute]
        [Required]
        public Guid accountId { get; set; }
        [FromBody]
        [Required]
        public MoneyChangeModel Withdrawal { get; set; }
        [FromHeader]
        public Guid? TraceId { get; set; }

        [FromHeader(Name = "Idempotency-Key")]
        public string IdempotencyKey { get; set; }
    }
}
