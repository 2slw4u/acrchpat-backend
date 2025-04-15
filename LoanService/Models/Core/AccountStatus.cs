using System.ComponentModel;
using System.Text.Json.Serialization;

namespace LoanService.Models.General;

[JsonConverter(typeof(JsonStringEnumConverter<AccountStatus>))]
public enum AccountStatus
{
    [Description("Счёт открыт")]
    Opened,
    [Description("Счёт закрыт пользователем")]
    Closed
}