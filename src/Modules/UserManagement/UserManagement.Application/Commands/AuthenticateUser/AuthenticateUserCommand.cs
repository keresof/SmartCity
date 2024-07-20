using MediatR;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Commands.AuthenticateUser;

public record AuthenticateUserCommand(string Email, string Password) : IRequest<AuthenticationResult>;