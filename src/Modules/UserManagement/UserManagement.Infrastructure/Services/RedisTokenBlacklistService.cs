using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using StackExchange.Redis;
using UserManagement.Application.Interfaces;

namespace UserManagement.Infrastructure.Services;

public class RedisTokenBlacklistService : ITokenBlacklistService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IConfiguration _configuration;
    private const string BlacklistPrefix = "blacklisted_token:";

    public RedisTokenBlacklistService(IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _redis = redis;
        _configuration = configuration;
    }

    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        var db = _redis.GetDatabase();
        return await db.KeyExistsAsync($"{BlacklistPrefix}{token}");
    }

    public async Task BlacklistTokenAsync(string token, TimeSpan expiryTimeSpan = default)
    {
        var db = _redis.GetDatabase();
        if (expiryTimeSpan == default)
        {
            var jsonWebToken = new JsonWebToken(token);
            var expiry = jsonWebToken.ValidTo;
            expiryTimeSpan = expiry - DateTime.UtcNow;
            // I mean, this shouldn't happen, but just in case
            if (expiryTimeSpan <= TimeSpan.Zero)
            {
                expiryTimeSpan = TimeSpan.FromMinutes(_configuration.GetValue<double>("JwtExpiry"));
            }
        }
        await db.StringSetAsync($"{BlacklistPrefix}{token}", "blacklisted", expiryTimeSpan);
    }
}