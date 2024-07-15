using MediatR;
using ReportManagement.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using ReportManagement.Domain.Enums;

namespace ReportManagement.Application.Commands.DeleteReport
{
    public class DeleteReportCommandHandler : IRequestHandler<DeleteReportCommand, Guid>
    {
        private readonly IReportRepository _reportRepository;

        public DeleteReportCommandHandler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Guid> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetByIdAsync(request.Id);

            if (report == null || report.UserId != request.UserId || report.Status == ReportStatus.InProgress)
            {
                throw new InvalidOperationException("Cannot delete this report.");
            }

            await _reportRepository.DeleteAsync(request.Id);
            return request.Id;
        }
    }
}