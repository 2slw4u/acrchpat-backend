using System.Text.Json.Serialization;

namespace LoanService.Models.Loan;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentStatus
{
    Payed,
    Overdue,
    NotYet
}