using Microsoft.AspNetCore.SignalR;
using ModernBaseProject.Infrastructure.Notifications.Hubs;

namespace ModernBaseProject.Infrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToAllAsync(string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
    }

    public async Task SendToUserAsync(string userId, string message)
    {
        await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", message);
    }
}