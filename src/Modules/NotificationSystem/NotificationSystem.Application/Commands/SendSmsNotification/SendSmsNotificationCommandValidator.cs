using FluentValidation;

namespace NotificationSystem.Application.Commands.SendSmsNotification;

public class SendSmsCommandValidator : AbstractValidator<SendSmsCommand>
{
    public SendSmsCommandValidator()
    {
        RuleFor(x => x.Recipient).NotEmpty().Matches(@"^\+[1-9]\d{1,14}$"); // E.164 format
        RuleFor(x => x.Content).NotEmpty().MaximumLength(160);
    }
}