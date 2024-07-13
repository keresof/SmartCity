using MediatR;
using System;

namespace ReportManagement.Application.Commands.DeleteReport
{
    public record DeleteReportCommand(Guid Id, Guid UserId) : IRequest<Guid>;
}