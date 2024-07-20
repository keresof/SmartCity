using MediatR;

namespace UserManagement.Application.Commands.LogoutUser;

public record LogoutUserCommand(string AccessToken) : IRequest<Unit>;