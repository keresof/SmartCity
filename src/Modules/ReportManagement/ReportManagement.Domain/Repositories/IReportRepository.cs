using ReportManagement.Domain.Entities;

namespace ReportManagement.Domain.Repositories;

public interface IReportRepository
{
    Task<Report?> GetByIdAsync(int id);

    Task AddAsync(Report? report);
    // Add other repository methods as needed
}