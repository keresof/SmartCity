using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;


namespace Shared.Infrastructure.Persistence
{
   public class ApplicationDbContext : DbContext
{
    private readonly IEnumerable<IModuleDbContext> _moduleContexts;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IEnumerable<IModuleDbContext> moduleContexts) : base(options)
    {
        _moduleContexts = moduleContexts;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var moduleContext in _moduleContexts)
        {
            moduleContext.ConfigureModelBuilder(modelBuilder);
        }
    }
}
}