using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Domain.Interfaces;

public interface INotificationServiceFactory
{
    INotificationService GetNotificationService(NotificationType notificationType);
}