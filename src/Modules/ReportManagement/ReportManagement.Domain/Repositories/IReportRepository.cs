// ReportManagement.Domain.Repositories.IReportRepository.cs
using ReportManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportManagement.Domain.Repositories
{
    public interface IReportRepository
    {
        Task<Report> GetByIdAsync(Guid id);
        Task AddAsync(Report report);
        Task UpdateAsync(Report report);
        Task DeleteAsync(Guid id);
        Task<List<Report>> SearchAsync(string? title, string? location);
        Task<List<Report>> GetByUserIdAsync(Guid userId);
    }
}