using Microsoft.Extensions.Logging;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Interfaces;

namespace NotificationSystem.Infrastructure.Services;

public class EmailService(ILogger<EmailService> logger) : INotificationService
{
    private readonly ILogger<EmailService> _logger = logger;
    public async Task SendAsync(Notification notification)
    {
        if (notification is not EmailNotification emailNotification)
        {
            throw new ArgumentException("Invalid notification type");
        }

        // Implement email sending logic
        Console.WriteLine($"Sending email to {emailNotification.Recipient}: {emailNotification.Subject}");
        _logger.LogInformation($"Sending email to {emailNotification.Recipient}: {emailNotification.Subject}");
        // In a real implementation, you would use an email service provider here
        await Task.CompletedTask;
    }
}