using NotificationSystem.Domain.Entities;

namespace NotificationSystem.Domain.Interfaces;

public interface INotificationService
{
    Task SendAsync(Notification notification);
}
