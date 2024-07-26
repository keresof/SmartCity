using FluentValidation;

namespace NotificationSystem.Application.Commands.SendEmailNotification;

public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
    {
        public SendEmailCommandValidator()
        {
            RuleFor(x => x.Recipient).NotEmpty().EmailAddress();
            RuleFor(x => x.Subject).NotEmpty();
            RuleFor(x => x.PlainTextContent).NotEmpty();
            RuleFor(x => x.HtmlContent).NotEmpty();
        }
    }