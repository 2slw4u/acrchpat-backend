using System.Text.Json.Serialization;

namespace LoanService.Models.General;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionResultStatus
{
    Success,
    Failure
}