using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NotificationService.Models;
[JsonConverter(typeof(JsonStringEnumConverter<TransactionType>))]
public enum TransactionType
{
    Deposit,
    Withdrawal,
    LoanAccrual,
    LoanPayment,
    Transfer
}