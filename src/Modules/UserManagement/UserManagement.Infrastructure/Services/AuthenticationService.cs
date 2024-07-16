using IdentityModel;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthenticationService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !user.PasswordHash.Verify(password))
        {
            return new AuthenticationResult { Success = false, Errors = ["Invalid credentials"] };
        }

        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _userRepository.UpdateAsync(user);

        return new AuthenticationResult
        {
            Success = true,
            Token = accessToken,
            RefreshToken = refreshToken
        };
    }

    public Task<AuthenticationResult> AuthenticateWithExternalProviderAsync(string provider, string token)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = _tokenService.ValidateToken(accessToken);
        if (principal == null)
        {
            return new AuthenticationResult { Success = false, Errors = ["Invalid access token"] };
        }

        var claimsPrincipal = await principal;
        var userId = Guid.Parse(claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value);

        var user = await _userRepository.GetByIdAsync(userId.ToString());
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new AuthenticationResult { Success = false, Errors = ["Invalid refresh token"] };
        }

        var newAccessToken = _tokenService.CreateAccessToken(user);
        var newRefreshToken = _tokenService.CreateRefreshToken();

        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        await _userRepository.UpdateAsync(user);

        return new AuthenticationResult
        {
            Success = true,
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    // Implement external provider authentication methods here
}