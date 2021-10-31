using System.Threading.Tasks;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR
{
    public class ChatService : IChatService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessage(string activityId, CommentDto comment)
        {
            await _hubContext.Clients.Group(activityId).SendAsync("newChatMessage", comment);
        }
    }
}