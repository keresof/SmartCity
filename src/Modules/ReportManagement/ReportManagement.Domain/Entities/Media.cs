using Shared.Common.Abstract;
using ReportManagement.Domain.Enums;

namespace ReportManagement.Domain.Entities;

public class Media : AuditableEntity
{
    public string Url { get; set; }
    public Guid ReportId { get; set; }
    public Guid UserId { get; set; }
}