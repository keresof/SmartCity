using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Persistence;

public abstract class ModuleDbContext : DbContext, IModuleDbContext
{
    protected ModuleDbContext(DbContextOptions options) : base(options) { }

    public abstract void ConfigureModelBuilder(ModelBuilder modelBuilder);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureModelBuilder(modelBuilder);
    }
}