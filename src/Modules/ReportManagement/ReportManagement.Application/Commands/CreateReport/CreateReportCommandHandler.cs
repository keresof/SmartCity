using MediatR;
using ReportManagement.Domain.Entities;
using ReportManagement.Domain.Enums;
using ReportManagement.Domain.Repositories;

namespace ReportManagement.Application.Commands.CreateReport;

public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, int>
{
    private readonly IReportRepository _reportRepository;

    public CreateReportCommandHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<int> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var report = new Report
        {
            Title = request.Title,
            Description = request.Description,
            Location = request.Location,
            Status = (ReportStatus)request.Status,
            MediaUrl = request.MediaUrl,
            UserId = request.UserId
        };

        report.SetCreatedBy(request.UserId.ToString());
        report.SetLastModifiedBy(request.UserId.ToString());

        await _reportRepository.AddAsync(report);

        return report.Id;
    }
}