using MediatR;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.Application.Commands.RefreshUserToken;

public class RefreshTokenCommandHandler (IAuthenticationService authenticationService) : IRequestHandler<RefreshUserTokenCommand, AuthenticationResult>
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<AuthenticationResult> Handle(RefreshUserTokenCommand request, CancellationToken cancellationToken)
    {
        return await _authenticationService.RefreshTokenAsync(request.RefreshToken);
    }
}