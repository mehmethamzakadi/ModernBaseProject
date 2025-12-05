using Microsoft.AspNetCore.SignalR;
using ModernBaseProject.Core.Constants;

namespace ModernBaseProject.Infrastructure.Notifications.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(JwtClaims.Sub)?.Value;
        if (userId != null)
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(JwtClaims.Sub)?.Value;
        if (userId != null)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        await base.OnDisconnectedAsync(exception);
    }
}
