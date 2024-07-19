using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using UserManagement.Application.Interfaces;

namespace UserManagement.Infrastructure.Auth;
public class OAuthProviderFactory : IOAuthProviderFactory
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Dictionary<string, Func<IOAuthProvider>> _providers;
    private readonly IConnectionMultiplexer _redis;

    public OAuthProviderFactory(IConfiguration configuration, IHttpClientFactory httpClientFactory, IConnectionMultiplexer redis)
    {
        _redis = redis;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _providers = new Dictionary<string, Func<IOAuthProvider>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Google"] = () => new GoogleOAuthProvider(_configuration, _httpClientFactory, _redis),
            ["Microsoft"] = () => new MicrosoftOAuthProvider(_configuration, _httpClientFactory, _redis)
        };
    }

    public IOAuthProvider GetProvider(string providerName)
    {
        if (_providers.TryGetValue(providerName, out var providerFunc))
        {
            return providerFunc();
        }
        throw new NotSupportedException($"Provider {providerName} is not supported.");
    }
}
