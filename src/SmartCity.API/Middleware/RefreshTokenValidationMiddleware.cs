

using UserManagement.Application.Interfaces;

namespace SmartCity.API.Middleware;

public class RefreshTokenValidationMiddleware(RequestDelegate next, IAuthenticationService authenticationService)
{
    private readonly RequestDelegate _next = next;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "sub");
            if (userIdClaim != null)
            {
                var userId = userIdClaim.Value;

                if (!await _authenticationService.HasValidRefreshToken(userId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid or expired refresh token");
                    return;
                }
            }
        }

        await _next(context);
    }

}

public static class RefreshTokenValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseRefreshTokenValidationMiddleware(this IApplicationBuilder builder)
    {
        return builder.Use(async (context, next) =>
        {
            var authenticationService = context.RequestServices.GetRequiredService<IAuthenticationService>();
            var middleware = new RefreshTokenValidationMiddleware(next, authenticationService);
            await middleware.InvokeAsync(context);
        });
    }
}