using MediatR;
using ReportManagement.Application.DTOs;
using ReportManagement.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReportManagement.Application.Queries.SearchReports
{
    public class SearchReportsQueryHandler : IRequestHandler<SearchReportsQuery, List<ReportDto>>
    {
        private readonly IReportRepository _reportRepository;

        public SearchReportsQueryHandler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<List<ReportDto>> Handle(SearchReportsQuery request, CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.SearchAsync(request.Title, request.Location);
            return reports.Select(report => new ReportDto(
                report.Id,
                report.Title,
                report.Description,
                report.Location,
                report.Status,
                report.Created,
                report.Medias,
                report.UserId,
                report.LastModified,
                report.LastModifiedBy,
                report.CreatedBy,
                report.Coordinates
            )).ToList();
        }
    }
}