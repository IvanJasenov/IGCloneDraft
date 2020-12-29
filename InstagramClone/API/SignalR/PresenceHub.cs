using API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;

        public PresenceHub(PresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            await _presenceTracker.UserConnected(Context.User.FindFirst(ClaimTypes.Name)?.Value, Context.ConnectionId);
            // Here si fetch the Username from the token as it is in HttpContextExtension.cs because in controllers I inherit from a diferent place
            await Clients.Others.SendAsync("UserIsOnline", Context.User.FindFirst(ClaimTypes.Name)?.Value);

            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _presenceTracker.UserDisconnected(Context.User.FindFirst(ClaimTypes.Name)?.Value, Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", Context.User.FindFirst(ClaimTypes.Name)?.Value);

            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnDisconnectedAsync(exception);

        }
    }
}
