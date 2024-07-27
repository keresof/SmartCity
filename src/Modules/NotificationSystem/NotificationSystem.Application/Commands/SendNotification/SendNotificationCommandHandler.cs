using MediatR;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Interfaces;

namespace NotificationSystem.Application.Commands.SendNotification;

public abstract class SendNotificationCommandHandler<TCommand>(INotificationServiceFactory notificationServiceFactory, INotificationRepository notificationRepository)
: IRequestHandler<TCommand, bool>
        where TCommand : SendNotificationCommand
{
    protected readonly INotificationServiceFactory _notificationServiceFactory = notificationServiceFactory;
    protected readonly INotificationRepository _notificationRepository = notificationRepository;

    public abstract Task<bool> Handle(TCommand request, CancellationToken cancellationToken);

    protected async Task<bool> SendNotification(Notification notification)
    {
        await _notificationRepository.SaveAsync(notification);

        try
        {
            var notificationService = _notificationServiceFactory.GetNotificationService(notification.Type);
            await notificationService.SendAsync(notification);
            notification.MarkAsSent();
            await _notificationRepository.SaveAsync(notification);
            return true;
        }
        catch (Exception)
        {
            notification.MarkAsFailed();
            await _notificationRepository.SaveAsync(notification);
            return false;
        }
    }
}