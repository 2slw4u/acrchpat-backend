using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CoreService.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter<AccountStatus>))]
    public enum CurrencyISO
    {
        [Description("Рубль")]
        RUB,
        [Description("Евро")]
        EUR,
        [Description("Доллар США")]
        USD
    }
}
