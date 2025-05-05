namespace NotificationService.Models
{
    public record PushTokenDto
    {
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
