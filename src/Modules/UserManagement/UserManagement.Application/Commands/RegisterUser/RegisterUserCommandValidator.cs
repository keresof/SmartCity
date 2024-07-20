using FluentValidation;

namespace UserManagement.Application.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required and must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Must(x => x.Any(char.IsDigit))
            .Must(x => x.Any(char.IsUpper))
            .Must(x => x.Any(char.IsLower))
            .Must(x => x.Any(char.IsPunctuation) || x.Any(char.IsSymbol) || x.Any(char.IsSeparator))
            .WithMessage("Password must be at least 8 characters and must contain at least one digit, one uppercase letter, one lowercase letter, and one special character.");
    }
}