using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.Infrastructure.Services;

public class OAuthService : IOAuthService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IConfiguration _configuration;
    private readonly IAuthenticationService _authenticationService;
    private readonly Dictionary<Type, IOAuthProvider> _providers;

    public OAuthService (
        IConnectionMultiplexer redis,
        IConfiguration configuration,
        IAuthenticationService authenticationService,
        IEnumerable<IOAuthProvider> providers)
    {
        _redis = redis;
        _configuration = configuration;
        _authenticationService = authenticationService;
        _providers = new Dictionary<Type, IOAuthProvider>();
        foreach (var provider in providers)
        {
            _providers[provider.GetType()] = provider;
        }
    }

    public async Task<string> BuildChallengeUrlAsync(string providerName, string redirectUri)
    {
        var provider = GetProviderByName(providerName);
        var state = RandomNumberGenerator.GetHexString(32);
        var challengeUrl = provider.BuildChallengeUrl(state, redirectUri);
        var db = _redis.GetDatabase();
        await db.StringSetAsync($"oauth_state:{state}", provider.Name, TimeSpan.FromMinutes(5));
        return challengeUrl;
    }

    public async Task<AuthenticationResult> HandleRemoteAuthAsync(string providerName, string code, string state, string redirectUri)
    {
        // Validate state
        var db = _redis.GetDatabase();
        var stateKey = $"oauth_state:{state}";
        var storedState = await db.StringGetAsync(stateKey);

        if (storedState.IsNullOrEmpty || !await db.KeyDeleteAsync(stateKey))
        {
            return new AuthenticationResult { Success = false, Errors = new[] { "Invalid state" } };
        }
        var storedProvider = storedState.ToString();
        if (!storedProvider.Equals(providerName, StringComparison.InvariantCultureIgnoreCase))
        {
            return new AuthenticationResult { Success = false, Errors = new[] { "Provider mismatch" } };
        }

        var provider = GetProviderByName(providerName);
                
        // Exchange code for token
        var tokenResponse = await provider.ExchangeCodeForTokenAsync(code, redirectUri);
        if (tokenResponse == null)
        {
            return new AuthenticationResult { Success = false, Errors = new[] { "Failed to exchange code for token" } };
        }

        // Get user info
        var userInfo = await provider.GetUserInfoAsync(tokenResponse);
        if (userInfo == null)
        {
            return new AuthenticationResult { Success = false, Errors = new[] { "Failed to get user info" } };
        }

        // Authenticate with your system
        return await _authenticationService.AuthenticateWithExternalProviderAsync(provider.Name, userInfo);
    }

    private IOAuthProvider GetProvider<T>() where T : IOAuthProvider
    {
        if (!_providers.TryGetValue(typeof(T), out var provider))
        {
            throw new InvalidOperationException($"Provider {typeof(T).Name} is not registered.");
        }
        return provider;
    }

    private IOAuthProvider GetProviderByName(string name)
    {
        var provider = _providers.Values.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (provider == null)
        {
            throw new InvalidOperationException($"Provider {name} is not registered.");
        }
        return provider;
    }
}