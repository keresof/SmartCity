using SmartCity.Core.Interfaces;

namespace SmartCity.Modules.ReportManagement.Application.Commands;

public class CreateReportCommand : IRequest<int>, MediatR.IRequest<int>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
}