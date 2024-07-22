using ReportManagement.Domain.Entities;

namespace ReportManagement.Domain.Repositories;

public interface IReportMediaRepository
{
    Task AddAsync(ReportMedia reportMedia);
    Task<ReportMedia?[]> GetByReportIdAsync(Guid reportId);
    Task<ReportMedia?> GetByIdAsync(Guid reportMediaId);
    Task UpdateAsync(ReportMedia reportMedia);
    Task DeleteAllFromReportAsync(Guid reportId);
    Task DeleteAsync(Guid reportMediaId);
}