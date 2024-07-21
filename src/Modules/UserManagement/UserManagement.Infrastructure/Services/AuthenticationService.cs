using IdentityModel;
using StackExchange.Redis;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConnectionMultiplexer _redis;
    private readonly IConfiguration _configuration;
    private readonly ITokenBlacklistService _tokenBlacklistService;

    public AuthenticationService(IUserRepository userRepository, ITokenService tokenService, IConnectionMultiplexer redis,IConfiguration configuration, ITokenBlacklistService tokenBlacklistService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _redis = redis;
        _configuration = configuration;
        _tokenBlacklistService = tokenBlacklistService;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || (user.HasPassword() && !user.PasswordHash.Verify(password)))
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
        await _userRepository.SaveChangesAsync();

        return new AuthenticationResult
        {
            Success = true,
            Token = accessToken,
            RefreshToken = refreshToken
        };
    }

   public async Task<AuthenticationResult> AuthenticateWithExternalProviderAsync(string provider, OAuthUserInfo userInfo)
    {
        var externalId = userInfo.Id;
        var email = userInfo.Email;
        var name = userInfo.Name;
        var firstName = userInfo.GivenName;
        var lastName = userInfo.Surname;

        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            user = User.Create(firstName ?? name, lastName, email);
            user.SetOAuthId(provider, externalId);
            await _userRepository.AddAsync(user);
        }
        else
        {
            if (!user.HasOAuthId(provider))
            {
                // Email is registered with a different provider or password
                // Controller should issue an OTP to add new login method to account
                return new AuthenticationResult { Success = false, Errors = new[] { "Email is already registered. Use a different login method or issue an OTP to add new login method to account." } };
            }
            if (user.GetOAuthId(provider) != externalId)
            {
                // Email is registered with a different external account
                return new AuthenticationResult { Success = false, Errors = new[] { "Email is already registered with a different external account from the same provider." } };
            }
            // Update the user's name and last name if not present
            user.SetFirstName(firstName ?? user.FirstName);
            user.SetLastName(lastName ?? user.LastName);
            await _userRepository.UpdateAsync(user);
        }

        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["RefreshTokenExpiry"])));
        await _userRepository.SaveChangesAsync();
        return new AuthenticationResult
        {
            Success = true,
            Token = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task LogoutAsync(string accessToken, string refreshToken)
    {
        var principal = await _tokenService.ValidateToken(accessToken);
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null || principal == null)
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        var id = principal.FindFirst(JwtClaimTypes.Subject)?.Value;
        if (id != user.Id.ToString())
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        // Add the refresh token to the blacklist
        await _tokenBlacklistService.BlacklistTokenAsync(accessToken);

        // Set the user's refreshToken to null
        user.SetRefreshToken(null, DateTime.MinValue);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
    {

        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new AuthenticationResult { Success = false, Errors = ["Invalid refresh token"] };
        }

        var newAccessToken = _tokenService.CreateAccessToken(user);
        var newRefreshToken = _tokenService.CreateRefreshToken();

        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(_configuration.GetValue<double>("RefreshTokenExpiry")));
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return new AuthenticationResult
        {
            Success = true,
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }


    public async Task<bool> HasValidRefreshToken(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null && user.RefreshToken != null && user.RefreshTokenExpiryTime > DateTime.UtcNow;
    }

}