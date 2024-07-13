using MediatR;
using System;

namespace ReportManagement.Application.Commands.UpdateReport
{
    public record UpdateReportCommand(Guid Id, string Title, string Description, string Location, int Status, string MediaUrl, Guid OfficerId) : IRequest<Guid>;
}