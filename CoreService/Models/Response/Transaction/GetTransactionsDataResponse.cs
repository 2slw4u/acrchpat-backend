using CoreService.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Response.Transaction
{
    public class GetTransactionsDataResponse
    {
        [Required]
        public List<TransactionDTO> Transactions { get; set; }
    }
}
