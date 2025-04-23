using System.Text.Json.Serialization;

namespace PreferenceService.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ThemeType
{
    Light,
    Dark
}