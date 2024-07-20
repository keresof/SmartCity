using UserManagement.Application.DTOs;

namespace UserManagement.Application.Interfaces;

public interface IOAuthService
{
    Task<string> BuildChallengeUrlAsync(string providerName, string redirectUri);
    Task<AuthenticationResult> HandleRemoteAuthAsync(string providerName, string code, string state, string redirectUri);
}