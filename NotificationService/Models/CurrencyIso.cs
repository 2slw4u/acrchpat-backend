using System.Text.Json.Serialization;

namespace NotificationService.Models;
    
[JsonConverter(typeof(JsonStringEnumConverter<CurrencyIso>))]
public enum CurrencyIso
{
    RUB,
    EUR,
    USD
}