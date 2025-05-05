
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Database;
using NotificationService.Models;
using NotificationService.Services;

[ApiController]
[Route("api")]
public class NotificationController(NotificationManager _notificationManager) : ControllerBase
{
    [HttpPost("test")]
    public async Task<IActionResult> SendTest([FromQuery] string token)
    {
        var msg = new Message
        {
            Token = token,
            Notification = new Notification
            {
                Title = "Привет !!1!",
                Body = "Мама!"
            },
            Webpush = new WebpushConfig
            {
                FcmOptions = new WebpushFcmOptions
                {
                    Link = "https://localhost:5173"
                }
            }
        };

        var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(msg);
        return Ok(new { messageId });
    }

    [HttpPost("push-tokens")]
    public async Task<IActionResult> Upsert([FromBody] PushTokenDto dto)
    {
        var userId = (Guid)HttpContext.Items["UserId"];

        var result = await _notificationManager.UpsertToken(userId, dto);

        return Ok(result);
    }

    [HttpDelete("push-tokens/{token}")]
    public async Task<IActionResult> Remove(string token)
    {
        var userId = (Guid)HttpContext.Items["UserId"];

        var result = await _notificationManager.RemoveToken(userId, token);
        
        return Ok(result);
    }

}