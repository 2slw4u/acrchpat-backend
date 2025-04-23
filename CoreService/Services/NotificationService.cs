using CoreService.Models.Database;
using CoreService.Models.Database.Entity;
using CoreService.Models.DTO;
using CoreService.Services.Interfaces;
using System.Net.WebSockets;
using System.Text;

namespace CoreService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly CoreDbContext _dbContext;
        public NotificationService(CoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateTransactionNotification(HttpContext context, TransactionDTO newTransaction)
        {
            /*_dbContext.Add(new TransactionEntity
            {
                
            });
            await _dbContext.SaveChangesAsync();*/
        }

        public async Task WebSocketEcho(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                var receivedUserId = Guid.Parse(Encoding.UTF8.GetString(buffer, 0, receiveResult.Count));
                var notifEntity = await _dbContext.Transactions
                    .Where(x => x.Account.UserId == receivedUserId && x.Status == NotificationStatus.New)
                    .OrderByDescending(x => x.Created)
                    .FirstOrDefaultAsync();

                if (notifEntity != null)
                {
                    string responseText = notifEntity.Text;
                    var responseMessage = new Notification
                    {
                        UserId = receivedUserId,
                        Text = responseText,
                    };

                    var jsonResponse = JsonConvert.SerializeObject(responseMessage);
                    var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

                    await webSocket.SendAsync(
                        new ArraySegment<byte>(responseBytes, 0, responseBytes.Length),
                        receiveResult.MessageType,
                        receiveResult.EndOfMessage,
                        CancellationToken.None);

                    notifEntity.Status = NotificationStatus.Sent;
                    await _dbContext.SaveChangesAsync();
                }

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);

        }
    }
