using MediatR;

namespace ReportManagement.Application.Commands.UpdateReport;

public record UpdateReportCommand(
    Guid Id,
    string Title,
    string Description,
    string[] Location,
    int Status,
    string[] MediaUrls,
    decimal[] Coordinates,
    Guid OfficerId) : IRequest<Guid>;