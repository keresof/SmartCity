using MediatR;
using NotificationSystem.Application.Commands.SendNotification;

namespace NotificationSystem.Application.Commands.SendEmailNotification;

public class SendEmailCommand : SendNotificationCommand
    {
        public string Subject { get; set; }
        public string PlainTextContent { get; set; }
        public string HtmlContent { get; set; }
    }