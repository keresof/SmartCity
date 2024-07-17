using IdentityModel;
using StackExchange.Redis;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;

namespace UserManagement.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConnectionMultiplexer _redis;
    private const string BlacklistPrefix = "blacklisted_token:";

    public AuthenticationService(IUserRepository userRepository, ITokenService tokenService, IConnectionMultiplexer redis)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _redis = redis;
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

    public async Task LogoutAsync(string token)
    {
        var principal = await _tokenService.ValidateToken(token);
        if (principal == null)
        {
            throw new InvalidOperationException("Invalid token");
        }

        var userId = Guid.Parse(principal.FindFirst(JwtClaimTypes.Subject)?.Value);
        var user = await _userRepository.GetByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Set the user's refreshToken to null
        user.SetRefreshToken(null, DateTime.MinValue);
        await _userRepository.UpdateAsync(user);

        // Blacklist the token
        await BlacklistTokenAsync(token);
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = await _tokenService.ValidateToken(accessToken);
        if (principal == null)
        {
            return new AuthenticationResult { Success = false, Errors = ["Invalid access token"] };
        }

        // Check if the token is blacklisted
        if (await IsTokenBlacklistedAsync(accessToken))
        {
            return new AuthenticationResult { Success = false, Errors = ["Token is blacklisted"] };
        }

        var userId = Guid.Parse(principal.FindFirst(JwtClaimTypes.Subject)?.Value);

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

    private async Task BlacklistTokenAsync(string token)
    {
        var jsonWebToken = new JsonWebToken(token);
        var expiry = jsonWebToken.ValidTo;
        var expiryTimeSpan = expiry - DateTime.UtcNow;

        var db = _redis.GetDatabase();
        await db.StringSetAsync($"{BlacklistPrefix}{token}", "blacklisted", expiryTimeSpan);
    }

    private async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        var db = _redis.GetDatabase();
        return await db.KeyExistsAsync($"{BlacklistPrefix}{token}");
    }

    // Implement external provider authentication methods here
}