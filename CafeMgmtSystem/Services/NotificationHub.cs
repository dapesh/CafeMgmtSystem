using Microsoft.AspNetCore.SignalR;

namespace CafeMgmtSystem.Services
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
