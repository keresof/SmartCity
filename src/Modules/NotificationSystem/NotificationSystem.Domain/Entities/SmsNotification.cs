using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Domain.Entities;

public class SmsNotification(string recipient, string content) : Notification(recipient, NotificationType.SMS)
    {
    public new string Content { get; private set; } = content;
}