using MediatR;

namespace UserManagement.Application.Commands.BuildOAuthChallengeUrl;

public class BuildOAuthChallengeUrlCommand : IRequest<string>
{
    public string ProviderName { get; set; }
    public string RedirectUri { get; set; }
}
