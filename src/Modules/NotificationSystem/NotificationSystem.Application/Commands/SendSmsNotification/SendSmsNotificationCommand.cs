using NotificationSystem.Application.Commands.SendNotification;

namespace NotificationSystem.Application.Commands.SendSmsNotification;

public class SendSmsCommand : SendNotificationCommand
{
    public string Content { get; set; }
}