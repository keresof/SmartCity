using ReportManagement.Domain.Enums;

namespace ReportManagement.Application.DTOs;

public record ReportDto(
    int Id,
    string Title,
    string Description,
    string Location,
    ReportStatus Status,
    DateTime Created,
    string MediaUrl,
    Guid UserId,
    DateTime? LastModified,
    string LastModifiedBy,
    string CreatedBy
);