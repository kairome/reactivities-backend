using System.Threading.Tasks;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR
{
    public class ChatService : SignalrSender<ChatHub>, IChatService
    {

        public ChatService(IHubContext<ChatHub> hubContext) : base(hubContext)
        {
        }

        public async Task SendMessage(string activityId, CommentDto comment)
        {
            await SendHubMessage(activityId, "newChatMessage", comment);
        }
    }
}