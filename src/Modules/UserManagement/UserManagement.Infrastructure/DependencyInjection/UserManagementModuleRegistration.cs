namespace UserManagement.Infrastructure.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common.Interfaces;
using Shared.Common.Utilities;
using UserManagement.Application.Commands.SendOTP;
using UserManagement.Application.Interfaces;
using UserManagement.Infrastructure.Persistence;
using UserManagement.Infrastructure.Repositories;

public class UserManagementModuleRegistration : IModuleRegistration
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = ConnectionStringParser.ConvertToNpgsqlFormat(configuration["DefaultConnection"]!);
        services.AddDbContext<UserManagementDbContext>(
            options => options.UseNpgsql(connectionString)
        )
        .AddScoped<IUserRepository, UserRepository>()
        .AddScoped<IOTPRepository, OTPRepository>()
        .AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(SendOTPCommand).Assembly);
        });

    }
}