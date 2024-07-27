using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Domain.Entities;

public class PushNotification : Notification
{
    public PushNotification() : base(string.Empty, NotificationType.Push)
    {
    }

    public PushNotification(string recipient, string title, string body, Dictionary<string, string>? data = null)
        : base(recipient, NotificationType.Push)
    {
        Title = title;
        Body = body;
        Data = data ?? [];
    }

    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string> Data { get; set; } = [];
}
