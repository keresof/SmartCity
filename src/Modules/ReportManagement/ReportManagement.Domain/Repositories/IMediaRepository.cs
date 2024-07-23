// ReportManagement.Domain.Repositories.IReportRepository.cs
using ReportManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportManagement.Domain.Repositories;
    public interface IMediaRepository
    {
        Task<Media?> GetByFileNameAsync(string fileName);
        Task AddAsync(Media media);
        Task DeleteAsync(string fileName);
        Task<List<Media>> GetByReportIdAsync(Guid reportId);
        Task<List<Media>> GetByUserIdAsync(Guid userId);
    }