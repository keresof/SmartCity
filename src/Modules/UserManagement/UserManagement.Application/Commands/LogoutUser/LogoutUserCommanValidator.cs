using FluentValidation;

namespace UserManagement.Application.Commands.LogoutUser;

public class LogoutUserCommandValidator : AbstractValidator<LogoutUserCommand>
{
    public LogoutUserCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token is required");
    }
}