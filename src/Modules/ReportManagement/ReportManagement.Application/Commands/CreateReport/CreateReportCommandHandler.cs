using MediatR;
using ReportManagement.Domain.Entities;
using ReportManagement.Domain.Enums;
using ReportManagement.Domain.Repositories;

namespace ReportManagement.Application.Commands.CreateReport;

public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, Guid>
{
    private readonly IReportRepository _reportRepository;

    public CreateReportCommandHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Guid> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var report = new Report
        {
            Title = request.Title,
            Description = request.Description,
            Location = request.Location,
            Status = (ReportStatus)request.Status,
            UserId = request.UserId,
            Coordinates = request.Coordinates
        };

        report.CreatedBy = request.UserId.ToString();
        report.Updated(request.UserId.ToString(), report.Created);

        await _reportRepository.AddAsync(report);

        return report.Id;
    }
}