using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
        public ChatHub()
        {
            
        }

        public override async Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            var activityId = context.Request.Query["activityId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
            await base.OnConnectedAsync();
        }
    }
}