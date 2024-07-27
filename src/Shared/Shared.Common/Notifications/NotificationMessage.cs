namespace Shared.Common.Notifications;
public class NotificationMessage
{
    public string Type { get; set; }
    public string Recipient { get; set; }
    public string? Subject { get; set; }
    public string Content { get; set; }
    public string? HtmlContent { get; set; }
    public IDictionary<string, string>? Data { get; set; }
}