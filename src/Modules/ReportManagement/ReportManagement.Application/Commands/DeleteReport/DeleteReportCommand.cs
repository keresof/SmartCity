using MediatR;
using System;

namespace ReportManagement.Application.Commands.DeleteReport
{
    public record DeleteReportCommand(int Id, Guid UserId) : IRequest<int>;
}