using CoreService.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.DTO
{
    public class TransactionResultDTO
    {
        [Required]
        public Guid TransactionId { get; set; }
        public Guid LoanId { get; set; }
        [Required]
        public Guid PaymentId { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public bool Status { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
