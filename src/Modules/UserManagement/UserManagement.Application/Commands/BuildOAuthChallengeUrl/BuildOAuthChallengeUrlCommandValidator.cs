using FluentValidation;

namespace UserManagement.Application.Commands.BuildOAuthChallengeUrl;

public class BuildOAuthChallengeUrlCommandValidator : AbstractValidator<BuildOAuthChallengeUrlCommand>
{
    public BuildOAuthChallengeUrlCommandValidator()
    {
        RuleFor(x => x.RedirectUri)
            .NotEmpty().WithMessage("RedirectUri is required")
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _)).WithMessage("ReturnUri is not valid");

        RuleFor(x => x.ProviderName.ToLowerInvariant())
            .NotEmpty().WithMessage("ProviderName is required")
            .Must(new[] { "google", "microsoft"}.Contains).WithMessage("ProviderName is not valid. Supported providers are google and microsoft");
    }
}