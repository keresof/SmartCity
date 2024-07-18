namespace UserManagement.Infrastructure.DependencyInjection;

using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Common.Behaviors;
using Shared.Common.Interfaces;
using Shared.Common.Utilities;
using Shared.Infrastructure.Redis;
using UserManagement.Application.Commands.RegisterUser;
using UserManagement.Application.Commands.SendOTP;
using UserManagement.Application.Interfaces;
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
        .AddScoped<IUserRepository, UserRepository>()
        .AddScoped<IRoleRepository, RoleRepository>()
        .AddScoped<IPermissionRepository, PermissionRepository>()
        .AddScoped<ITokenService, JwtTokenService>(s =>
        {
            return new JwtTokenService(configuration);
        })
        .AddScoped<IOTPService, RedisOTPService>(s =>
        {

            return new RedisOTPService(redis);
        })
        .AddScoped<IAuthenticationService, AuthenticationService>(s =>
        {
            return new AuthenticationService(s.GetRequiredService<IUserRepository>(), s.GetRequiredService<ITokenService>(), redis, s.GetRequiredService<IHttpContextAccessor>(), configuration);
        })
        .AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly)
        .AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(SendOTPCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(ValidationBehavior<,>).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        var tokenService = services.BuildServiceProvider().GetRequiredService<ITokenService>();
        Console.WriteLine("RSA Public Key: ");
        Console.WriteLine(((JwtTokenService)tokenService).RsaSecurityKey.ToString());
        services.AddAuthentication(
            opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
        .AddJwtBearer(opts =>
        {
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

        services.AddAuthentication()
            .AddGoogle(opts =>{
                opts.ClientId = configuration["AuthGoogleClientId"];
                opts.ClientSecret = configuration["AuthGoogleClientSecret"];
            })
            .AddFacebook(opts =>{
                opts.AppId = configuration["AuthFacebookAppId"];
                opts.AppSecret = configuration["AuthFacebookAppSecret"];
            })
            .AddMicrosoftAccount(opts =>{
                opts.ClientId = configuration["AuthMicrosoftClientId"];
                opts.ClientSecret = configuration["AuthMicrosoftClientSecret"];
            });
        services.AddAuthorization();

    }
}