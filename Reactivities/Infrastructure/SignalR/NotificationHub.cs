using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public NotificationHub()
        {
            
        }
        
        public override async Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            var userId = context.Request.Query["userId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            await base.OnConnectedAsync();
        }
    }
}