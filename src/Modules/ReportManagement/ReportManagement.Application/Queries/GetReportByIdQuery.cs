using MediatR;
using ReportManagement.Application.DTOs;

namespace ReportManagement.Application.Queries.GetReportById;

public record GetReportByIdQuery(Guid Id) : IRequest<ReportDto>;