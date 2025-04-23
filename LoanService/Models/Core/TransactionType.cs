using System.ComponentModel;
using System.Text.Json.Serialization;

namespace LoanService.Models.General;
[JsonConverter(typeof(JsonStringEnumConverter<TransactionType>))]
public enum TransactionType
{
    Deposit,
    Withdrawal,
    LoanAccrual,
    LoanPayment,
    Transfer
}