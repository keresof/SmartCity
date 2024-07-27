namespace Shared.Common.Notifications;

public interface INotificationSender
{
    Task SendEmailAsync(string recipient, string subject, string plainTextContent, string htmlContent);
    Task SendSmsAsync(string recipient, string content);
    Task SendPushNotificationAsync(string recipient, string title, string body, IDictionary<string, string>? data = null);
}