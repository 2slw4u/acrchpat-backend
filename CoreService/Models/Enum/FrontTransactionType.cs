using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CoreService.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter<FrontTransactionType>))]
    public enum FrontTransactionType
    {
        [Description("Withdrawal of funds through ATM")]
        Deposit,
        [Description("Deposit of funds through ATM")]
        Withdrawal,
        [Description("Money accrued from taking a loan")]
        LoanAccrual,
        [Description("Payment for a taken loan")]
        LoanPayment,
        [Description("Transer of money from the account")]
        TransferFrom,
        [Description("Transer of money to the account")]
        TransferTo
    }
}
