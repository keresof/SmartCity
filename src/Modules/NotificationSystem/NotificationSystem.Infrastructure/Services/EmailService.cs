using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Interfaces;

namespace NotificationSystem.Infrastructure.Services;

public class EmailService : INotificationService
{
    public async Task SendAsync(Notification notification)
    {
        if (notification is not EmailNotification emailNotification)
        {
            throw new ArgumentException("Invalid notification type");
        }

        // Implement email sending logic
        Console.WriteLine($"Sending email to {emailNotification.Recipient}: {emailNotification.Subject}");
        // In a real implementation, you would use an email service provider here
        await Task.CompletedTask;
    }
}