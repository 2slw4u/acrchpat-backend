using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RabbitConsumer
{
    public class Transaction
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

    [JsonConverter(typeof(JsonStringEnumConverter<TransactionType>))]
    public enum TransactionType
    {
        [Description("Withdrawal of funds through ATM")]
        Deposit,
        [Description("Deposit of funds through ATM")]
        Withdrawal
    }
}
