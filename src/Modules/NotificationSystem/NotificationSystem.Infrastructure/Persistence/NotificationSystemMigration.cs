using Microsoft.EntityFrameworkCore;
using Shared.Common;

namespace NotificationSystem.Infrastructure.Persistence;

public class NotificationSystemMigration : ICanMigrate
{
    public void ApplyMigrations(IServiceProvider? services)
    {
        var dbContext = services?.GetService(typeof(NotificationDbContext)) as NotificationDbContext;
        dbContext?.Database.Migrate();
    }
}