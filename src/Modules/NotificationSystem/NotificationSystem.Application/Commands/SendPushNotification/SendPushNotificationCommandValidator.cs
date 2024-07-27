using FluentValidation;

namespace NotificationSystem.Application.Commands.SendPushNotification;

public class SendPushNotificationCommandValidator : AbstractValidator<SendPushNotificationCommand>
{
    public SendPushNotificationCommandValidator()
    {
        RuleFor(x => x.Recipient).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(200);
    }
}