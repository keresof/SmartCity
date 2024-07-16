using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string email, string password);
    Task<AuthenticationResult> AuthenticateWithExternalProviderAsync(string provider, string token);
    Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
}