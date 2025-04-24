using System.Text.Json.Serialization;

namespace LoanService.Models.Core;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionResultStatus
{
    Success,
    Failure
}