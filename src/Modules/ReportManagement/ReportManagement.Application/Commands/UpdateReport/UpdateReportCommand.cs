using MediatR;

namespace ReportManagement.Application.Commands.UpdateReport;

public record UpdateReportCommand(
    Guid Id,
    string Title,
    string Description,
    string[] Location,
    int Status,
    string MediaUrl,
    decimal[] Coordinates,
    Guid OfficerId) : IRequest<Guid>;