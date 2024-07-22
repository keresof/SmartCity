using Shared.Common.Abstract;
using ReportManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportManagement.Domain.Entities;

public class ReportMedia : AuditableEntity
{
    [Column(TypeName = "bytea")]
    public byte[] Media { get; set; }
    public string MimeType { get; set; }
    public string? Filename { get; set; }
    public Guid ReportId { get; set; }
}