using ReportManagement.Domain.Entities;
using ReportManagement.Domain.Repositories;
using Shared.Infrastructure;

namespace ReportManagement.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Report?> GetByIdAsync(int id)
    {
        return await _context.Reports.FindAsync(id);
    }

    public async Task AddAsync(Report? report)
    {
        await _context.Reports.AddAsync(report);
        await _context.SaveChangesAsync();
    }

    // Implement other repository methods
}