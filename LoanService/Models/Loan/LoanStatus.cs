using System.Text.Json.Serialization;

namespace LoanService.Models.Loan;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LoanStatus
{
    Open,
    Closed
}