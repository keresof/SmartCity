namespace SmartCity.Core.Interfaces;

// SmartCity.Core/Interfaces/IUnitOfWork.cs
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}