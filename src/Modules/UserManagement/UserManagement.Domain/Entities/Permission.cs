using Shared.Common.Abstract;

namespace UserManagement.Domain.Entities;
public class Permission : AuditableEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<RolePermission> Roles { get; private set; } = new List<RolePermission>();

    private Permission() { } // For EF Core

    public static Permission Create(string name, string description)
    {
        return new Permission
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };
    }
}