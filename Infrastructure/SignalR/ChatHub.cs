using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ICurrentUserIdentity _currentUserIdentity;
        private readonly Dictionary<string, List<string>> _connections = new();
        public ChatHub(ICurrentUserIdentity currentUserIdentity)
        {
            _currentUserIdentity = currentUserIdentity;
        }

        public override async Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            var activityId = context.Request.Query["activityId"];
            var currentUser = await _currentUserIdentity.GetCurrentUser();

            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);

            var existingUsers = _connections.GetValueOrDefault(activityId) ?? new List<string>();
            
            existingUsers.Add(currentUser.Id);
            _connections.Add(activityId, existingUsers);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var currentUser = await _currentUserIdentity.GetCurrentUser();
            var activityId =  Context.GetHttpContext().Request.Query["activityId"];
            var existingUsers = _connections.GetValueOrDefault(activityId);
            existingUsers?.Remove(currentUser.Id);

            _connections[activityId] = existingUsers;

            await base.OnDisconnectedAsync(exception);
        }

        public Dictionary<string, List<string>> GetConnections()
        {
            return _connections;
        }
    }
}