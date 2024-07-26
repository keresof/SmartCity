using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationSystem.Application.Commands.SendNotification;
using NotificationSystem.Domain.Interfaces;
using NotificationSystem.Infrastructure.Persistence;
using NotificationSystem.Infrastructure.Services;
using Shared.Common.Interfaces;
using Shared.Common.Utilities;

namespace NotificationSystem.Infrastructure.DependencyInjection;

public class NotificationSystemModuleRegistration : IModuleRegistration
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SendNotificationCommand).Assembly))
        .AddDbContext<NotificationDbContext>(cfg => {
            cfg.UseNpgsql(
                ConnectionStringParser.ConvertToNpgsqlFormat(configuration["DefaultConnection"])
            );
        })
        .AddScoped<INotificationServiceFactory, NotificationServiceFactory>()
        .AddScoped<INotificationRepository, NotificationRepository>()
        .AddScoped<EmailService>()
        .AddScoped<SmsService>()
        .AddScoped<PushNotificationService>();
    }
}
