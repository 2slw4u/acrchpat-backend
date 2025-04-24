using System.Text.Json.Serialization;

namespace LoanService.Models.General;

[JsonConverter(typeof(JsonStringEnumConverter<CurrencyIso>))]
public enum CurrencyIso
{
    RUB,
    EUR,
    USD
}