using MediatR;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Commands.HandleOAuthCallback;

public class HandleOAuthCallbackCommand : IRequest<AuthenticationResult>
{
    public string ProviderName { get; set; }
    public string Code { get; set; }
    public string State { get; set; }
    public string RedirectUri { get; set; }
}