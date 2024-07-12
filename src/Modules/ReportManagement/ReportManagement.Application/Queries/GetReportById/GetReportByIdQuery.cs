using MediatR;
using ReportManagement.Application.DTOs;

namespace ReportManagement.Application.Queries.GetReportById;

public record GetReportByIdQuery(int Id) : IRequest<ReportDto>;