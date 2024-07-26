using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Domain.Entities;
public class EmailNotification(string recipient, string subject, string plainTextContent, string htmlContent)
: Notification(recipient, NotificationType.Email)
{
    public string Subject { get; private set; } = subject;
    public string PlainTextContent { get; private set; } = plainTextContent;
    public string HtmlContent { get; private set; } = htmlContent;
}