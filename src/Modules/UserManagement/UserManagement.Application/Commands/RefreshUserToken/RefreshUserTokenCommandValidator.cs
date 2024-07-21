using FluentValidation;

namespace UserManagement.Application.Commands.RefreshUserToken;

public class RefreshUserTokenCommandValidator : AbstractValidator<RefreshUserTokenCommand>
{
    public RefreshUserTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}