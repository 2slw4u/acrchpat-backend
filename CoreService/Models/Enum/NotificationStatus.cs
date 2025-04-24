using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CoreService.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter<NotificationStatus>))]
    public enum NotificationStatus
    {
        [Description("Нотификация готова к отправлению")]
        Created,
        [Description("Нотификация отправлена клиенту")]
        Sent
    }
}
