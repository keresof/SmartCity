using NotificationSystem.Application.Commands.SendNotification;

namespace NotificationSystem.Application.Commands.SendPushNotification;

public class SendPushNotificationCommand : SendNotificationCommand
{
    public string Title { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Data { get; set; }
}