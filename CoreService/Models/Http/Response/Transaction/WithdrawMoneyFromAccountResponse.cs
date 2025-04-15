using CoreService.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Response.Transaction
{
    public class WithdrawMoneyFromAccountResponse
    {
        [Required]
        public TransactionDTO NewWithdrawalTransaction { get; set; }
    }
}
