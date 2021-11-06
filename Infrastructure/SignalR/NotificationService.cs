using System.Threading.Tasks;
using Application.Core;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR
{
    public class NotificationService : SignalrSender<NotificationHub>, INotificationService
    {

        public NotificationService(IHubContext<NotificationHub> hubContext) : base(hubContext)
        {
        }

        public async Task SendMessage(string userId, string msgId, object message)
        {
            await SendHubMessage(userId, msgId, message);
        }
    }
}