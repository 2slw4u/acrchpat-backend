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
        public Guid AccountId { get; set; }
        [Required]
        public AccountEntity Account { get; set; }
        public Guid? DestinationAccountId { get; set; }
        public AccountEntity? DestinationAccount { get; set; }
        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public double Amount { get; set; }
        public double? DestinationAmount { get; set; }
        [Required]
        public TransactionType Type { get; set; }
        [Required]
        [DefaultValue(CurrencyISO.RUB)]
        public CurrencyISO Currency { get; set; }
        public CurrencyISO? DestinationCurrency { get; set; }
        [Required]
        public DateTime PerformedAt { get; set; }
    }
}
