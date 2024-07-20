using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.Infrastructure.Auth;

public class MicrosoftOAuthProvider : IOAuthProvider
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConnectionMultiplexer _redis;
    public string Name => "Microsoft";

    public MicrosoftOAuthProvider(IConfiguration configuration, IHttpClientFactory httpClientFactory, IConnectionMultiplexer redis)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _redis = redis;
    }

    public string BuildChallengeUrl(string state, string redirectUri)
    {
        var clientId = _configuration["OAuth:Microsoft:ClientId"];
        var scope = "openid email profile";
        return $"https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id={clientId}&response_type=code&scope={scope}&redirect_uri={Uri.EscapeDataString(redirectUri)}&state={state}";
    }

    public async Task<OAuthTokenResponse> ExchangeCodeForTokenAsync(string code, string redirectUri)
    {
        var clientId = _configuration["OAuth:Microsoft:ClientId"];
        var clientSecret = _configuration["OAuth:Microsoft:ClientSecret"];

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync("https://login.microsoftonline.com/common/oauth2/v2.0/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = redirectUri
        }));

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<OAuthTokenResponse>(content);
    }

    public async Task<OAuthUserInfo> GetUserInfoAsync(OAuthTokenResponse token)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var response = await client.GetAsync("https://graph.microsoft.com/v1.0/me");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<OAuthUserInfo>(content);
    }
}