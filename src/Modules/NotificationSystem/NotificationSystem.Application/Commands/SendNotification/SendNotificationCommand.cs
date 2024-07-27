using MediatR;

namespace NotificationSystem.Application.Commands.SendNotification;

public abstract class SendNotificationCommand : IRequest<bool>
{
    public string Recipient { get; set; }
}