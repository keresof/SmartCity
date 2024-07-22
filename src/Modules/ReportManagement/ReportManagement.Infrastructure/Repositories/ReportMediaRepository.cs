namespace ReportManagement.Infrastructure.Repositories;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReportManagement.Domain.Entities;
using ReportManagement.Domain.Repositories;
using ReportManagement.Infrastructure.Persistence;

public class ReportMediaRepository(ReportManagementDbContext dbContext) : IReportMediaRepository
{
    private readonly ReportManagementDbContext _dbContext = dbContext;
    public async Task AddAsync(ReportMedia reportMedia)
    {
        await _dbContext.AddAsync(reportMedia);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAllFromReportAsync(Guid reportId)
    {
        var reports = _dbContext.ReportMedias.Where(rm => rm.ReportId == reportId);
        _dbContext.RemoveRange(reports);
        await _dbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(Guid reportMediaId)
    {
        _dbContext.Remove(reportMediaId);
        return _dbContext.SaveChangesAsync();
    }

    public Task<ReportMedia?> GetByIdAsync(Guid reportMediaId)
    {
        return _dbContext.ReportMedias.FirstOrDefaultAsync(rm => rm.Id == reportMediaId);
    }

    public async Task<ReportMedia?[]> GetByReportIdAsync(Guid reportId)
    {
        return await _dbContext.ReportMedias.Where(rm => rm.ReportId == reportId).ToArrayAsync();
    }

    public Task UpdateAsync(ReportMedia reportMedia)
    {
        _dbContext.Update(reportMedia);
        return _dbContext.SaveChangesAsync();
    }
}

