using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CoreService.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter<TransactionType>))]
    public enum TransactionType
    {
        [Description("Withdrawal of funds through ATM")]
        Deposit = 0,
        [Description("Deposit of funds through ATM")]
        Withdrawal = 1,
        [Description("Money accrued from taking a loan")]
        LoanAccrual = 2,
        [Description("Payment for a taken loan")]
        LoanPayment = 3
    }
}
