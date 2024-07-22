// ReportManagement.Domain.Entities.Report.cs
using Shared.Common.Abstract;
using ReportManagement.Domain.Enums;

namespace ReportManagement.Domain.Entities
{
    public class Report : AuditableEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Location { get; set; } = [];
        public ReportStatus Status { get; set; }
        public IEnumerable<ReportMedia> Medias { get; set; } = [];
        public Guid UserId { get; set; }
        
        public decimal[] Coordinates { get; set; } = [];
    }
}