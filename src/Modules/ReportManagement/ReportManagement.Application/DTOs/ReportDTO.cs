using ReportManagement.Domain.Enums;

namespace ReportManagement.Application.DTOs;

public record ReportDto(
    Guid Id,
    string Title,
    string Description,
    string[] Location,
    ReportStatus Status,
    DateTime Created,
    string[] MediaUrls,
    Guid UserId,
    DateTime? LastModified,
    string LastModifiedBy,
    string CreatedBy,
    decimal[] Coordinates
);