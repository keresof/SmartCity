using Microsoft.EntityFrameworkCore;
namespace Shared.Infrastructure.Persistence;
public interface IModuleDbContext
{
    void ConfigureModelBuilder(ModelBuilder modelBuilder);
}