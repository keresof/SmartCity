using ReportManagement.Domain.Entities;

namespace ReportManagement.Domain.Repositories;

public interface IReportRepository
{
    Task<Report> GetByIdAsync(int id);
    Task AddAsync(Report report);
    Task<List<Report>> SearchAsync(string? title, string? description);
    Task<List<Report>> GetByUserIdAsync(string userId); // New method to get reports by user ID
}