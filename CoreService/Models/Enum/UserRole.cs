using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CoreService.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter<UserRole>))]
    public enum UserRole
    {
        [Description("An usual customer")]
        Client,
        [Description("A bank employee")]
        Employee
    }
}
