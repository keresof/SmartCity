using NotificationSystem.Application.Commands.SendNotification;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Interfaces;

namespace NotificationSystem.Application.Commands.SendSmsNotification;

public class SendSmsCommandHandler(INotificationServiceFactory notificationServiceFactory, INotificationRepository notificationRepository) 
: SendNotificationCommandHandler<SendSmsCommand>(notificationServiceFactory, notificationRepository)
{
    public override async Task<bool> Handle(SendSmsCommand request, CancellationToken cancellationToken)
    {
        var notification = new SmsNotification(request.Recipient, request.Content);
        return await SendNotification(notification);
    }
}