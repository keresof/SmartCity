using MediatR;
using UserManagement.Application.Interfaces;

namespace UserManagement.Application.Commands.LogoutUser;

public class LogoutUserCommandHandler(IAuthenticationService authenticationService) : IRequestHandler<LogoutUserCommand, Unit>
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<Unit> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        await _authenticationService.LogoutAsync(request.AccessToken);
        return Unit.Value;
    }
}