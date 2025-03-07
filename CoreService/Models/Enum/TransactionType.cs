using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CoreService.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter<TransactionType>))]
    public enum TransactionType
    {
        [Description("Withdrawal of funds through ATM")]
        Deposit,
        [Description("Deposit of funds through ATM")]
        Withdrawal
    }
}
