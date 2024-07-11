using Microsoft.EntityFrameworkCore;
using SmartCity.Core.Interfaces;

namespace SmartCity.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for your entities will be added here

    public DbSet<TEntity> Set<TEntity>() where TEntity : class
    {
        throw new NotImplementedException();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}