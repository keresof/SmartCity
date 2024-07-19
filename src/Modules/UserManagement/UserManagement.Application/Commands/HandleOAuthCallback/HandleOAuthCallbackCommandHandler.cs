using System.ComponentModel;
using System.Text.Json;
using MediatR;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.Application.Commands.HandleOAuthCallback;

public class HandleOAuthCallbackCommandHandler : IRequestHandler<HandleOAuthCallbackCommand, AuthenticationResult>
{
    private readonly IOAuthService _oauthService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IOAuthProviderFactory _providerFactory;

    public HandleOAuthCallbackCommandHandler(IOAuthProviderFactory providerFactory, IAuthenticationService authenticationService, IOAuthService oAuthService)
    {
        _providerFactory = providerFactory;
        _authenticationService = authenticationService;
        _oauthService = oAuthService;
    }

    public async Task<AuthenticationResult> Handle(HandleOAuthCallbackCommand request, CancellationToken cancellationToken)
    {
        return await _oauthService.HandleRemoteAuthAsync(request.ProviderName, request.Code, request.State, request.RedirectUri);
    }
}