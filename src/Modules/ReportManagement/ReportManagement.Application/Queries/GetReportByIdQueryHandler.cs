// ReportManagement.Application.Queries.GetReportById.GetReportByIdQueryHandler.cs
using MediatR;
using ReportManagement.Application.DTOs;
using ReportManagement.Domain.Repositories;
using Shared.Common.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace ReportManagement.Application.Queries.GetReportById
{
    public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ReportDto>
    {
        private readonly IReportRepository _reportRepository;

        public GetReportByIdQueryHandler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<ReportDto> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetByIdAsync(request.Id);
            if (report == null)
            {
                throw new NotFoundException($"Report with ID {request.Id} not found.");
            }

            return new ReportDto(
                report.Id,
                report.Title,
                report.Description,
                report.Location,
                report.Status,
                report.Created,
                report.MediaUrl,
                report.UserId,
                report.LastModified,
                report.LastModifiedBy,
                report.CreatedBy
            );
        }
    }
}