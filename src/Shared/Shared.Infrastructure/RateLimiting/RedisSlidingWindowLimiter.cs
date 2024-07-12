using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Shared.Infrastructure.RateLimiting
{
    public class RedisSlidingWindowLimiter
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisSlidingWindowLimiter(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<bool> TryAcquireAsync(string key, TimeSpan windowDuration, int maxRequests)
        {
            var db = _redis.GetDatabase();
            var now = DateTime.UtcNow.Ticks;
            var windowStart = now - windowDuration.Ticks;

            var transaction = db.CreateTransaction();
            
            // Remove old entries
            transaction.SortedSetRemoveRangeByScoreAsync(key, 0, windowStart, Exclude.None);
            
            // Add current request
            transaction.SortedSetAddAsync(key, now.ToString(), now);
            
            // Get count of requests in the current window
            var countTask = transaction.SortedSetLengthAsync(key, double.NegativeInfinity, double.PositiveInfinity, Exclude.None);
            
            // Set expiry on the key
            transaction.KeyExpireAsync(key, windowDuration);

            await transaction.ExecuteAsync();

            var count = await countTask;

            return count <= maxRequests;
        }
    }
}