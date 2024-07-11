using SmartCity.Core.Entities;
namespace SmartCity.Modules.ReportManagement.Domain;

public class Report : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Location { get; set; }
}

public enum ReportStatus
{
    New,
    InProgress,
    Resolved,
    Closed
}