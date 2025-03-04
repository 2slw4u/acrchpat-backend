using CoreService.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CoreService.Models.Database.Entity
{
    public class TransactionEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public AccountEntity Account { get; set; }
        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public double Amount { get; set; }
        [Required]
        public TransactionType Type { get; set; }
        [Required]
        public DateTime PerformedAt { get; set; }
    }
}
