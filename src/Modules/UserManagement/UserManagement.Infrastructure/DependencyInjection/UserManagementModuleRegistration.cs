namespace UserManagement.Infrastructure.DependencyInjection;

using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Shared.Common.Behaviors;
using Shared.Common.Interfaces;
using Shared.Common.Utilities;
using Shared.Infrastructure.Redis;
using StackExchange.Redis;
using UserManagement.Application.Commands.RegisterUser;
using UserManagement.Application.Commands.SendOTP;
using UserManagement.Application.Interfaces;
using UserManagement.Infrastructure.Auth;
using UserManagement.Infrastructure.Persistence;
using UserManagement.Infrastructure.Repositories;
using UserManagement.Infrastructure.Services;

public class UserManagementModuleRegistration : IModuleRegistration
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = ConnectionStringParser.ConvertToNpgsqlFormat(configuration["DefaultConnection"]!);
        var redis = RedisConnectionHelper.Connection;

        services.AddDbContext<UserManagementDbContext>(
            options => options.UseNpgsql(connectionString)
        )
        .AddScoped<ITokenBlacklistService, RedisTokenBlacklistService>(s =>
        {
            return new RedisTokenBlacklistService(redis, configuration);
        })
        .AddScoped<IUserRepository, UserRepository>()
        .AddScoped<IRoleRepository, RoleRepository>()
        .AddScoped<IPermissionRepository, PermissionRepository>()
        .AddScoped<ITokenService, JwtTokenService>(s =>
        {
            return new JwtTokenService(configuration, s.GetRequiredService<ILogger<JwtTokenService>>());
        })
        .AddScoped<IOTPService, RedisOTPService>(s =>
        {

            return new RedisOTPService(redis);
        })
        .AddHttpClient()
        .AddSingleton<IOAuthProviderFactory, OAuthProviderFactory>(s => {
            return new OAuthProviderFactory(configuration, s.GetRequiredService<IHttpClientFactory>(), redis);
        })
        
        .AddScoped<IAuthenticationService, AuthenticationService>(s =>
        {
            return new AuthenticationService(s.GetRequiredService<IUserRepository>(), s.GetRequiredService<ITokenService>(), redis, configuration, s.GetRequiredService<ITokenBlacklistService>());
        })
        .AddScoped<IOAuthProvider, GoogleOAuthProvider>( s=> {
            return new GoogleOAuthProvider(configuration, s.GetRequiredService<IHttpClientFactory>(), redis);
        })
        .AddScoped<IOAuthProvider, MicrosoftOAuthProvider>(s => {
            return new MicrosoftOAuthProvider(configuration, s.GetRequiredService<IHttpClientFactory>(), redis);
        })
        .AddScoped<IOAuthService, OAuthService>(s =>{
            return new OAuthService(
                redis,
                configuration,
                s.GetRequiredService<IAuthenticationService>(),
                s.GetServices<IOAuthProvider>()
            );
        })
        .AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly)
        .AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(SendOTPCommand).Assembly);
        });

        var tokenService = services.BuildServiceProvider().GetRequiredService<ITokenService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opts =>
        {
            opts.MapInboundClaims = false;
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtIssuer"],
                ValidAudience = configuration["JwtAudience"],
                IssuerSigningKey = ((JwtTokenService)tokenService).RsaSecurityKey,
                ClockSkew = TimeSpan.Zero
            };

        });

        services.AddAuthorization();

    }
}