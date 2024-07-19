using UserManagement.Application.DTOs;

namespace UserManagement.Application.Interfaces;

public interface IOAuthProvider
{
    string Name { get; }
    string BuildChallengeUrl(string state, string redirectUri);
    Task<OAuthTokenResponse> ExchangeCodeForTokenAsync(string code, string redirectUri);
    Task<OAuthUserInfo> GetUserInfoAsync(OAuthTokenResponse token);
}