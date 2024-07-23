using MediatR;
using ReportManagement.Domain.Enums;
using ReportManagement.Domain.Repositories;
using Shared.Common.Exceptions;

namespace ReportManagement.Application.Commands.UpdateReport;

public class UpdateReportCommandHandler : IRequestHandler<UpdateReportCommand, Guid>
{
    private readonly IReportRepository _reportRepository;

    public UpdateReportCommandHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Guid> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetByIdAsync(request.Id);

        if (report == null) throw new NotFoundException($"Report with ID {request.Id} not found.");

        report.Title = request.Title;
        report.Description = request.Description;
        report.Location = request.Location;
        report.Status = (ReportStatus)request.Status;
        report.Coordinates = request.Coordinates;
        report.Updated(request.OfficerId.ToString());

        await _reportRepository.UpdateAsync(report);

        return report.Id;
    }
}