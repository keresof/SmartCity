using MediatR;
using SmartCity.Core.Interfaces;

using SmartCity.Modules.ReportManagement.Application.Commands;
using SmartCity.Modules.ReportManagement.Domain;

public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, int>
{
    private readonly IRepository<Report> _reportRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReportCommandHandler(IRepository<Report> reportRepository, IUnitOfWork unitOfWork)
    {
        _reportRepository = reportRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var report = new Report
        {
            Title = request.Title,
            Description = request.Description,
            Status = ReportStatus.New,
            CreatedAt = DateTime.UtcNow,
            Location = request.Location
        };

        await _reportRepository.AddAsync(report);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return report.Id;
    }
// TODO:
    public Task<int> Handle(CreateReportCommand request)
    {
        throw new NotImplementedException();
    }
}