using MediatR;

namespace UserManagement.Application.Commands.LogoutUser;

public record LogoutUserCommand(string AccessToken, string RefreshToken) : IRequest<Unit>;