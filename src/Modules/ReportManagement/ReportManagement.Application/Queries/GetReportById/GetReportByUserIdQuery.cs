// ReportManagement.Application.Queries.GetReportsByUserId.GetReportsByUserIdQuery.cs
using MediatR;
using ReportManagement.Application.DTOs;
using System.Collections.Generic;

namespace ReportManagement.Application.Queries.GetReportsByUserId
{
    public record GetReportsByUserIdQuery(string UserId) : IRequest<List<ReportDto>>;
}