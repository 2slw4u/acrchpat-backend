using CoreService.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Response.Transaction
{
    public class DepositMoneyToAccountResponse
    {
        [Required]
        public TransactionDTO NewDepositTransaction { get; set; }
    }
}
