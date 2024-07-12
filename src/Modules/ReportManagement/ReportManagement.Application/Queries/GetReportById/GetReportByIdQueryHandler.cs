using MediatR;
using ReportManagement.Application.DTOs;
using ReportManagement.Domain.Repositories;

namespace ReportManagement.Application.Queries.GetReportById;

public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ReportDto>
{
    private readonly IReportRepository _reportRepository;

    public GetReportByIdQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<ReportDto> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetByIdAsync(request.Id);
        if (report == null) return null;

        return new ReportDto(
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
        );
    }
}