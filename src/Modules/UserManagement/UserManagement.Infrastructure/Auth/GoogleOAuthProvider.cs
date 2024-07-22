using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.Infrastructure.Auth;

public class GoogleOAuthProvider : IOAuthProvider
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConnectionMultiplexer _redis;
    private const string GoogleJwksUrl = "https://www.googleapis.com/oauth2/v3/certs";
    private const string RedisKeyPrefix = "google_oauth_jwks:";
    private const int CacheExpirationInMinutes = 60;

    public string Name => "Google";



    public GoogleOAuthProvider(IConfiguration configuration, IHttpClientFactory httpClientFactory, IConnectionMultiplexer redis)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _redis = redis;
    }

    public string BuildChallengeUrl(string state, string redirectUri)
    {
        var clientId = _configuration["AuthGoogleClientId"];
        var scope = "openid+email+profile";
        return $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&response_type=code&scope={scope}&redirect_uri={new IdnMapping().GetAscii(_configuration["GoogleRedirectUri"])}&state={state}";
    }

    public async Task<OAuthTokenResponse> ExchangeCodeForTokenAsync(string code, string redirectUri)
    {
        var clientId = _configuration["AuthGoogleClientId"];
        var clientSecret = _configuration["AuthGoogleClientSecret"];

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = new IdnMapping().GetAscii(redirectUri)
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
        return await ExtractUserInfo(token.IdToken);
    }

    private async Task<OAuthUserInfo> ExtractUserInfo(string token)
    {
        var handler = new JsonWebTokenHandler();

        var jwks = await GetGoogleJwksAsync();


        Console.WriteLine("Unverified token: " + token);


        var result = await handler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://accounts.google.com",
            ValidateAudience = true,
            ValidAudience = _configuration["AuthGoogleClientId"],
            ValidateLifetime = true,
            IssuerSigningKeys = jwks.GetSigningKeys(),
            ValidateIssuerSigningKey = true,
            NameClaimType = "sub"
        });

        if (!result.IsValid)
        {
            return null;
        }

        return new OAuthUserInfo
        {
            Id = result.Claims.FirstOrDefault(c => c.Key == "sub").Value?.ToString(),
            Email = result.Claims.FirstOrDefault(c => c.Key == "email").Value?.ToString(),
            Name = result.Claims.FirstOrDefault(c => c.Key == "name").Value?.ToString(),
            GivenName = result.Claims.FirstOrDefault(c => c.Key == "given_name").Value?.ToString(),
            Surname = result.Claims.FirstOrDefault(c => c.Key == "family_name").Value?.ToString(),
            Picture = result.Claims.FirstOrDefault(c => c.Key == "picture").Value?.ToString(),
        };
    }

    private async Task<JsonWebKeySet> GetGoogleJwksAsync()
    {
        var db = _redis.GetDatabase();
        var cachedJwks = await db.StringGetAsync(RedisKeyPrefix + "jwks");

        if (!cachedJwks.IsNullOrEmpty)
        {
            return JsonSerializer.Deserialize<JsonWebKeySet>(cachedJwks);
        }

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(GoogleJwksUrl);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var jwks = JsonSerializer.Deserialize<JsonWebKeySet>(content);

        await db.StringSetAsync(RedisKeyPrefix + "jwks", content, TimeSpan.FromMinutes(CacheExpirationInMinutes));

        return jwks;
    }

}