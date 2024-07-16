using Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace ReportManagement.Infrastructure.Persistence;

public class ReportManagementModuleMigration : ICanMigrate
{
    public void ApplyMigrations(IServiceProvider? services)
    {
        var dbContext = services?.GetService(typeof(ReportManagementDbContext)) as ReportManagementDbContext;
        dbContext?.Database.Migrate();
    }
}