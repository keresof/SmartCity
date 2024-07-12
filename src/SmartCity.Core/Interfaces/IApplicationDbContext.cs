using Microsoft.EntityFrameworkCore;

namespace SmartCity.Core.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}