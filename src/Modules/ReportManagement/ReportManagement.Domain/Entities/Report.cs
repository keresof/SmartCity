using ReportManagement.Domain.Enums;

namespace ReportManagement.Domain.Entities;

public class Report
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}