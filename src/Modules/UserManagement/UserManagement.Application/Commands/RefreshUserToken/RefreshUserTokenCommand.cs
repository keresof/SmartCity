using MediatR;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Commands.RefreshUserToken;

public record RefreshUserTokenCommand (string RefreshToken) : IRequest<AuthenticationResult>;