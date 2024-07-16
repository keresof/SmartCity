using Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Infrastructure.Persistence;
public class UserManagementModuleMigration : ICanMigrate
{
    public void ApplyMigrations(IServiceProvider? services)
    {
        var dbContext = services?.GetService(typeof(UserManagementDbContext)) as UserManagementDbContext;
        dbContext?.Database.Migrate();
    }
}