// ReportManagement.Infrastructure.Repositories.ReportRepository.cs

using Microsoft.EntityFrameworkCore;
using ReportManagement.Domain.Entities;
using ReportManagement.Domain.Repositories;
using ReportManagement.Infrastructure.Persistence;

namespace ReportManagement.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ReportManagementDbContext _context;

    public ReportRepository(ReportManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Report> GetByIdAsync(int id)
    {
        return await _context.Reports.FindAsync(id);
    }

    public async Task AddAsync(Report report)
    {
        await _context.Reports.AddAsync(report);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Report>> SearchAsync(string? title, string? description)
    {
        return await _context.Reports
            .Where(r => (string.IsNullOrEmpty(title) || r.Title.Contains(title)) &&
                        (string.IsNullOrEmpty(description) || r.Description.Contains(description)))
            .ToListAsync();
    }

    public async Task<List<Report>> GetByUserIdAsync(string userId)
    {
        return await _context.Reports
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }
}