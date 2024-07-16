using MediatR;

namespace ReportManagement.Application.Commands.CreateReport;

public record CreateReportCommand(
    string Title,
    string Description,
    string[] Location,
    int Status,
    string MediaUrl,
    decimal[] Coordinates,
    Guid UserId) : IRequest<Guid>;