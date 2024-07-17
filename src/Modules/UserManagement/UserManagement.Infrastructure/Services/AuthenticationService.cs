using IdentityModel;
using StackExchange.Redis;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UserManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Services;

public class AuthenticationService : Application.Interfaces.IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConnectionMultiplexer _redis;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private const string BlacklistPrefix = "blacklisted_token:";

    public AuthenticationService(IUserRepository userRepository, ITokenService tokenService, IConnectionMultiplexer redis, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _redis = redis;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !user.PasswordHash.Verify(password))
        {
            return new AuthenticationResult { Success = false, Errors = ["Invalid credentials"] };
        }

        if(!user.AuthenticationMethods.Any(m => m == AuthenticationMethod.EmailPassword)){
            return new AuthenticationResult { Success = false, Errors = ["Email is already registered. Use a different login method or issue an OTP to add new login method to account."] };
        }

        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(_configuration.GetValue<int>("RefreshTokenExpiry")));
        await _userRepository.UpdateAsync(user);

        return new AuthenticationResult
        {
            Success = true,
            Token = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthenticationResult> AuthenticateWithExternalProviderAsync(string provider, string token)
    {
        var authenticateResult = await AuthenticateTokenAsync(provider, token);
        
        if (!authenticateResult.Succeeded)
        {
            return new AuthenticationResult { Success = false, Errors = ["Invalid external token"] };
        }

        var externalId = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
        var name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);
        var firstName = authenticateResult.Principal.FindFirstValue(ClaimTypes.GivenName);
        var lastName = authenticateResult.Principal.FindFirstValue(ClaimTypes.Surname);

        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            user = User.Create(firstName?? name, lastName, email);
            user.SetOAuthId(provider, externalId);
            await _userRepository.AddAsync(user);
        }
        else
        {
            if(!user.HasOAuthId(provider)){
                // Email is registered with a different provider or password
                // Controller should issue an OTP to add new login method to account
                return new AuthenticationResult { Success = false, Errors = ["Email is already registered. Use a different login method or issue an OTP to add new login method to account."] };
            }
            if (user.GetOAuthId(provider) != externalId)
            {
                // Email is registered with a different external account
                return new AuthenticationResult { Success = false, Errors = ["Email is already registered with a different external account from the same provider."] };
            }
            // Update the user's name and last name if not present
            user.SetFirstName(firstName?? user.FirstName);
            user.SetLastName(lastName?? user.LastName);
            await _userRepository.UpdateAsync(user);
        }
        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(Convert.ToUInt32(_configuration["RefreshTokenExpiry"])));
        await _userRepository.UpdateAsync(user);

        return new AuthenticationResult
        {
            Success = true,
            Token = accessToken,
            RefreshToken = refreshToken
        };
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

        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(_configuration.GetValue<int>("RefreshTokenExpiry")));
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

    private async Task<AuthenticateResult> AuthenticateTokenAsync(string provider, string token)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        httpContext.Request.Headers["Authorization"] = $"Bearer {token}";

        AuthenticateResult result = provider.ToLower() switch
        {
            "google" => await httpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme),
            "microsoft" => await httpContext.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme),
            "facebook" => await httpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme),
            _ => throw new NotSupportedException($"Provider {provider} is not supported.")
        };

        return result;
    }

}