using CoreService.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.Response.Support
{
    public class GetClientTransactionHistoryResponse
    {
        [Required]
        public List<DetailedTransactionDTO> Transactions { get; set; }
    }
}
