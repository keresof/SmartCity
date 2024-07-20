using System.Text.Json;
using System.Web;
using MediatR;
using UserManagement.Application.Interfaces;

namespace UserManagement.Application.Commands.BuildOAuthChallengeUrl;

public class BuildOAuthChallengeUrlCommandHandler : IRequestHandler<BuildOAuthChallengeUrlCommand, string>
{
    private readonly IOAuthService _oauthService;

    public BuildOAuthChallengeUrlCommandHandler(IOAuthService oAuthService)
    {
        _oauthService = oAuthService;
    }

    public async Task<string> Handle(BuildOAuthChallengeUrlCommand request, CancellationToken cancellationToken)
    {
        var challengeUrl = await _oauthService.BuildChallengeUrlAsync(request.ProviderName, request.RedirectUri);
        var redirectUrl = new Uri(challengeUrl);
        return redirectUrl.ToString();
    }
}
