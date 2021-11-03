using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR
{
    public class SignalrSender<THub> where THub : Hub
    {
        private readonly IHubContext<THub> _hubContext;

        public SignalrSender(IHubContext<THub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected async Task SendHubMessage(string groupId, string messageId, object message)
        {
            await _hubContext.Clients.Group(groupId).SendAsync(messageId, message);
        }
    }
}