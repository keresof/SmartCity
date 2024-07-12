using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Infrastructure.RateLimiting;
using StackExchange.Redis;

namespace SmartCity.API.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RateLimitAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _key;
        private readonly int _maxRequests;
        private readonly int _durationSeconds;

        public RateLimitAttribute(string key, int maxRequests, int durationSeconds)
        {
            _key = key;
            _maxRequests = maxRequests;
            _durationSeconds = durationSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var redis = context.HttpContext.RequestServices.GetService(typeof(ConnectionMultiplexer)) as ConnectionMultiplexer;

            if (redis == null)
            {
                await next();
                return;
            }

            var userId = context.HttpContext.User.Identity.Name ?? "anonymous";
            var key = $"{_key}:{userId}";

            var limiter = new RedisSlidingWindowLimiter(redis);

            if (await limiter.TryAcquireAsync(key, TimeSpan.FromSeconds(_durationSeconds), _maxRequests))
            {
                await next();
            }
            else
            {
                context.Result = new StatusCodeResult(429);
            }
        }
    }
}