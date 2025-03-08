using CoreService.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Request.Transaction
{
    public class WithdrawMoneyFromAccountRequest
    {
        [FromRoute]
        [Required]
        public Guid accountId { get; set; }
        [FromBody]
        [Required]
        public MoneyChangeModel Withdrawal { get; set; }
    }
}
