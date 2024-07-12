// ReportManagement.Application.Queries.SearchReports.SearchReportsQuery.cs
using MediatR;
using ReportManagement.Application.DTOs;
using System.Collections.Generic;

namespace ReportManagement.Application.Queries.SearchReports
{
    public record SearchReportsQuery(string? Title, string? Description) : IRequest<List<ReportDto>>;
}