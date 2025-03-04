using CoreService.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace CoreService.Models.DTO
{
    public class TransactionDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public double Amount { get; set; }
        [Required]
        public TransactionType Type { get; set; }
        [Required]
        public DateTime PerformedAt { get; set; }
    }
}
