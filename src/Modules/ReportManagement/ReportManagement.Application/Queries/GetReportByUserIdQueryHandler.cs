using MediatR;
using ReportManagement.Application.DTOs;
using ReportManagement.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReportManagement.Application.Queries.GetReportsByUserId
{
    public class GetReportsByUserIdQueryHandler : IRequestHandler<GetReportsByUserIdQuery, List<ReportDto>>
    {
        private readonly IReportRepository _reportRepository;

        public GetReportsByUserIdQueryHandler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<List<ReportDto>> Handle(GetReportsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetByUserIdAsync(request.UserId);
            return reports.Select(report => new ReportDto(
                report.Id,
                report.Title,
                report.Description,
                report.Location,
                report.Status,
                report.Created,
                report.MediaUrl,
                report.UserId,
                report.LastModified,
                report.LastModifiedBy,
                report.CreatedBy,
                report.Coordinates
            )).ToList();
        }
    }
}