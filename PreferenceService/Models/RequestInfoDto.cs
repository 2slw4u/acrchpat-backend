namespace PreferenceService.Models;

public class RequestInfoDto
{
    public Guid Id { get; set; }
    public DateTime ReceiveTime { get; set; }
    public string Protocol { get; set; } = "";
    public string Method { get; set; } = "";
    public string Host { get; set; } = "";
    public string Path { get; set; } = "";
    public Dictionary<string, string>? QueryParams { get; set; }
    public string? Body { get; set; }
}