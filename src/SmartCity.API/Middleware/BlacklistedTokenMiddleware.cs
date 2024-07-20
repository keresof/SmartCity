using UserManagement.Application.Interfaces;

namespace SmartCity.API.Middleware;

public class BlacklistedTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITokenBlacklistService _blacklistService;

    public BlacklistedTokenMiddleware(RequestDelegate next, ITokenBlacklistService blacklistService)
    {
        _next = next;
        _blacklistService = blacklistService ?? throw new ArgumentNullException(nameof(blacklistService));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            if (await _blacklistService.IsTokenBlacklistedAsync(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is blacklisted");
                return;
            }
        }

        await _next(context);
    }
}

public static class BlacklistedTokenMiddlewareExtensions
{
    public static IApplicationBuilder UseBlacklistedTokenMiddleware(this IApplicationBuilder builder)
    {
        return builder.Use(async (context, next) =>
        {
            var blacklistService = context.RequestServices.GetRequiredService<ITokenBlacklistService>();
            var middleware = new BlacklistedTokenMiddleware(next, blacklistService);
            await middleware.InvokeAsync(context);
        });
    }
}