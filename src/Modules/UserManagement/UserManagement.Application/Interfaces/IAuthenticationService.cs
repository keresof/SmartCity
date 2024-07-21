using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string email, string password);
    Task<AuthenticationResult> AuthenticateWithExternalProviderAsync(string provider, OAuthUserInfo userInfo);
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);
    Task<bool> HasValidRefreshToken(string userId);
    Task LogoutAsync(string accessToken, string refreshToken);
}