using NotificationSystem.Application.Commands.SendNotification;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Interfaces;

namespace NotificationSystem.Application.Commands.SendPushNotification;

public class SendPushNotificationCommandHandler(INotificationServiceFactory notificationServiceFactory, INotificationRepository notificationRepository) 
: SendNotificationCommandHandler<SendPushNotificationCommand>(notificationServiceFactory, notificationRepository)
{
    public override async Task<bool> Handle(SendPushNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = new PushNotification(request.Recipient, request.Title, request.Body, request.Data);
        return await SendNotification(notification);
    }
}