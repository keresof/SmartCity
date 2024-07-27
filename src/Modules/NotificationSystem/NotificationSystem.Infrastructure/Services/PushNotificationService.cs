using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Interfaces;

namespace NotificationSystem.Infrastructure.Services;

public class PushNotificationService : INotificationService
{
    public async Task SendAsync(Notification notification)
    {
        if (notification is not PushNotification pushNotification)
        {
            throw new ArgumentException("Invalid notification type");
        }

        // Implement push notification sending logic
        Console.WriteLine($"Sending push notification to {pushNotification.Recipient}: {pushNotification.Title}");
        // In a real implementation, you would use a push notification service provider here
        await Task.CompletedTask;
    }
}