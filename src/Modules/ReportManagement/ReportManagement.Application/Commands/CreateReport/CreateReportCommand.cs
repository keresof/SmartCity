using MediatR;

namespace ReportManagement.Application.Commands.CreateReport;

public record CreateReportCommand(string Title, string Description, string Location) : IRequest<int>;