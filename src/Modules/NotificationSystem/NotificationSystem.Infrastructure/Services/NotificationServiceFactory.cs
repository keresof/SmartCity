using Microsoft.Extensions.DependencyInjection;
using NotificationSystem.Domain.Enums;
using NotificationSystem.Domain.Interfaces;

namespace NotificationSystem.Infrastructure.Services;

public class NotificationServiceFactory : INotificationServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public INotificationService GetNotificationService(NotificationType type)
    {
        return type switch
        {
            NotificationType.Email => _serviceProvider.GetRequiredService<EmailService>(),
            NotificationType.SMS => _serviceProvider.GetRequiredService<SmsService>(),
            NotificationType.Push => _serviceProvider.GetRequiredService<PushNotificationService>(),
            _ => throw new ArgumentException($"Invalid notification type: {type}")
        };
    }
}