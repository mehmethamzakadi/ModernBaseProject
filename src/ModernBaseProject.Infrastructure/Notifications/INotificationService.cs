namespace ModernBaseProject.Infrastructure.Notifications;

public interface INotificationService
{
    Task SendToAllAsync(string message);
    Task SendToUserAsync(string userId, string message);
}