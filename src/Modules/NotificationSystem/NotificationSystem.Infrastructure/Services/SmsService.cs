using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Interfaces;

namespace NotificationSystem.Infrastructure.Services;

public class SmsService : INotificationService
{
    public async Task SendAsync(Notification notification)
    {
        if (notification is not SmsNotification smsNotification)
        {
            throw new ArgumentException("Invalid notification type");
        }

        // Implement SMS sending logic
        Console.WriteLine($"Sending SMS to {smsNotification.Recipient}: {smsNotification.Content}");
        // In a real implementation, you would use an SMS service provider here
        await Task.CompletedTask;
    }
}