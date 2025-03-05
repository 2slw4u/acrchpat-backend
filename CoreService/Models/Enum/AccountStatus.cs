using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CoreService.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter<AccountStatus>))]
    public enum AccountStatus
    {
        [Description("Счёт открыт")]
        Opened,
        [Description("Счёт закрыт пользователем")]
        Closed
    }
}
