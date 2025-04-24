using System.Text.Json.Serialization;

namespace LoanService.Models.General;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CurrencyIso
{
    RUB,
    EUR,
    USD
}