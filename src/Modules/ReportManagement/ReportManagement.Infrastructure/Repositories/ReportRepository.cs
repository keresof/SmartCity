// ReportManagement.Infrastructure.Repositories.ReportRepository.cs
using Microsoft.EntityFrameworkCore;
using ReportManagement.Domain.Entities;
using ReportManagement.Domain.Repositories;
using ReportManagement.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Infrastructure;

namespace ReportManagement.Infrastructure.Repositories
{

    public class ReportRepository : IReportRepository
    {
        private readonly ReportManagementDbContext _context;

        public ReportRepository(ReportManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Report> GetByIdAsync(Guid id)
        {
            return await _context.Reports.FindAsync(id);
        }

        public async Task AddAsync(Report report)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Report report)
        {
            _context.Reports.Update(report);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report != null)
            {
                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Report>> SearchAsync(string? title, string? location)
        {
            return await _context.Reports
                .Where(r => (string.IsNullOrEmpty(title) || r.Title.Contains(title)) &&
                            (string.IsNullOrEmpty(location) || r.Location.Contains(location)))
                .ToListAsync();
        }

        public async Task<List<Report>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Reports
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
    }
}