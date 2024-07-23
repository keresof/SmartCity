
using Microsoft.EntityFrameworkCore;
using ReportManagement.Domain.Entities;
using ReportManagement.Domain.Repositories;
using ReportManagement.Infrastructure.Persistence;

namespace ReportManagement.Infrastructure.Repositories;

public class MediaRepository(ReportManagementDbContext context) : IMediaRepository
{
    private readonly ReportManagementDbContext _context = context;
    public async Task AddAsync(Media media)
    {
        await _context.Media.AddAsync(media);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string fileName)
    {
        var media = await _context.Media.FirstOrDefaultAsync(m => m.Url == fileName);
        if (media != null)
        {
            _context.Media.Remove(media);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Media?> GetByFileNameAsync(string fileName)
    {
        return await _context.Media.AsNoTracking().FirstOrDefaultAsync(m => m.Url == fileName);
    }

    public async Task<List<Media>> GetByReportIdAsync(Guid reportId)
    {
        return await _context.Media.Where(m => m.ReportId.Equals(reportId)).ToListAsync();
    }

    public async Task<List<Media>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Media.Where(m => m.UserId.Equals(userId)).ToListAsync();
    }
}