using System.Globalization;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using NotificationService.Database;
using NotificationService.Exceptions;
using NotificationService.Integrations;
using NotificationService.Models;

namespace NotificationService.Services;

public class NotificationManager
{
    private readonly AppDbContext _context;
    private readonly ILogger<NotificationManager> _logger;

    public NotificationManager(AppDbContext context, ILogger<NotificationManager> logger)
    {
        _context = context;
        _logger = logger;
    }


    public async Task SendMessageAsync(TransactionResult result)
    {
        var clientTokens = await _context.UserPushTokens
            .Where(t => t.UserId == result.UserId && t.Role == "Client")
            .Select(t => t.FcmToken)
            .ToListAsync();

        var employeeTokens = await _context.UserPushTokens
            .Where(t => t.Role == "Employee")
            .Select(t => t.FcmToken)
            .ToListAsync();

        var messages = clientTokens.Select(t => BuildMsg(t, result, false))
                       .Concat(employeeTokens.Select(t => BuildMsg(t, result, true)))
                       .ToList();

        _logger.LogInformation(messages.Count.ToString());

        if (!messages.Any())
        {
            _logger.LogInformation("No recipient {userId}", result.UserId);
            return;
        }

        var badTokens = new List<string>();

        foreach (var batch in messages.Chunk(500))
        {
            var resp = await FirebaseMessaging.DefaultInstance.SendEachAsync(batch);

            badTokens.AddRange(
                resp.Responses
                    .Select((r, i) => new { r, i })
                    .Where(x => !x.r.IsSuccess &&
                                x.r.Exception is FirebaseMessagingException f &&
                                f.MessagingErrorCode == MessagingErrorCode.Unregistered)
                    .Select(x => batch[x.i].Token));
        }

        _logger.LogInformation("Actually we've sent the notification and are genuinely hoping it reaches u <3 {userId}", result.UserId);

        if (badTokens.Count > 0)
        {
            _context.UserPushTokens
                     .RemoveRange(_context.UserPushTokens
                     .Where(t => badTokens.Contains(t.FcmToken)));

            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted {Count} dead push tokens", badTokens.Count);
        }
    }

    private static Message BuildMsg(string token, TransactionResult result, bool toEmployee)
    {

        var body = toEmployee
            ? $"{result.AccountId}: {result.Type} {result.Amount:N2}"
            : $"{result.Type} " +
              $"{result.Amount:N2}";

        var data = new Dictionary<string, string?>
        {
            ["accountId"] = result.AccountId?.ToString(),
            ["amount"] = result.Amount.ToString("F2", CultureInfo.InvariantCulture),
            ["type"] = result.Type.ToString(),
            ["paymentId"] = result.PaymentId?.ToString(),
            ["loanId"] = result.LoanId?.ToString()
        };

        var cleanData = data.Where(kv => kv.Value is not null)
                            .ToDictionary(kv => kv.Key, kv => kv.Value!);

        return new Message
        {
            Token = token,
            Notification = new Notification
            {
                Title = "Новая транзакция",
                Body = body
            },
            Data = cleanData
        };
    }

    public async Task<ResponseModel> UpsertToken(Guid userId, PushTokenDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Token))
            throw new BadRequestException("Token is empty");

        var row = await _context.UserPushTokens
                          .SingleOrDefaultAsync(t => t.FcmToken == dto.Token);

        if (row is null)
        {
            _context.UserPushTokens.Add(new UserPushToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Role = dto.Role,
                FcmToken = dto.Token,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            row.UserId = userId;
            row.Role = dto.Role;
        }

        await _context.SaveChangesAsync();

        return new ResponseModel
        {
            Status = "Success",
            Message = "Token upserted successfully."
        };
    }

    public async Task<ResponseModel> RemoveToken(Guid userId, string token)
    {
        var row = await _context.UserPushTokens
                          .SingleOrDefaultAsync(t => t.UserId == userId &&
                                                     t.FcmToken == token);
        if (row is null)
        {
            throw new BadRequestException("Token configuration does not match any existing one.");
        }

        _context.UserPushTokens.Remove(row);

        await _context.SaveChangesAsync();

        return new ResponseModel{
            Status = "Success", 
            Message = "Token removed successfully."
        };
    }
}
