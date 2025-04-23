using CoreService.Models.DTO;
using System.Net.WebSockets;

namespace CoreService.Services.Interfaces
{
    public interface INotificationService
    {
        Task CreateTransactionNotification(HttpContext context, TransactionDTO newTransaction);
        Task WebSocketEcho(WebSocket webSocket);
    }
}
