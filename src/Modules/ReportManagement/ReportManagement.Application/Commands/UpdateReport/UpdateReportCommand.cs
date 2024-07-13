using MediatR;
using System;

namespace ReportManagement.Application.Commands.UpdateReport
{
    public record UpdateReportCommand(int Id, string Title, string Description, string Location, int Status, string MediaUrl, Guid OfficerId) : IRequest<int>;
}