using CoreService.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Http.Response.Support
{
    public class GetAccountTransactionHistoryResponse
    {
        [Required]
        public List<DetailedTransactionDTO> Transactions { get; set; }
    }
}
