namespace UserManagement.Infrastructure.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common.Interfaces;
using Shared.Common.Utilities;
using Shared.Infrastructure.Redis;
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
        .AddScoped<ITokenService, JwtTokenService>(s => {
            return new JwtTokenService(configuration);
        })
        .AddScoped<IOTPService, RedisOTPService>(s =>{
            
            return new RedisOTPService(redis);
        })
        .AddScoped<IAuthenticationService, AuthenticationService>(s => {
            return new AuthenticationService(s.GetRequiredService<IUserRepository>(), s.GetRequiredService<ITokenService>(), redis);
        })
        .AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(SendOTPCommand).Assembly);
        });

    }
}