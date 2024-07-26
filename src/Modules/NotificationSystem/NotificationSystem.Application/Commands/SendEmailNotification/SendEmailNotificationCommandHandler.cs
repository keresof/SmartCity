using NotificationSystem.Application.Commands.SendNotification;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Interfaces;

namespace NotificationSystem.Application.Commands.SendEmailNotification;

public class SendEmailCommandHandler
(
    INotificationServiceFactory notificationServiceFactory,
    INotificationRepository notificationRepository
)
: SendNotificationCommandHandler<SendEmailCommand>(notificationServiceFactory, notificationRepository)
{
    public override async Task<bool> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        var notification = new EmailNotification(request.Recipient, request.Subject, request.PlainTextContent, request.HtmlContent);
        return await SendNotification(notification);
    }
}