using MediatR;
using ReportManagement.Application.DTOs;
using System.Collections.Generic;

namespace ReportManagement.Application.Queries.SearchReports
{
    public record SearchReportsQuery(string? Title, string? Location) : IRequest<List<ReportDto>>;
}