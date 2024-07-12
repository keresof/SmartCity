// ReportManagement.Application.Queries.SearchReports.SearchReportsQueryHandler.cs

using MediatR;
using ReportManagement.Application.DTOs;
using ReportManagement.Application.Queries.SearchReports;
using ReportManagement.Domain.Repositories;

namespace ReportManagement.Application.Queries.GetReportById
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
            var reports = await _reportRepository.SearchAsync(request.Title, request.Description);
            return reports.Select(report => new ReportDto(
                report.Id,
                report.Title,
                report.Description,
                report.Location,
                report.Status,
                report.Created,
                report.MediaUrl,
                report.CreatedBy,
                report.LastModified,
                report.LastModifiedBy
            )).ToList();
        }
    }
}