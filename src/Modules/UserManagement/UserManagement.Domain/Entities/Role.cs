using Shared.Common.Abstract;

namespace UserManagement.Domain.Entities;

public class Role : AuditableEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<UserRole> Users { get; private set; } = new List<UserRole>();
    public List<RolePermission> Permissions { get; private set; } = new List<RolePermission>();

    private Role() { } // For EF Core

    public static Role Create(string name, string description)
    {
        return new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };
    }

    public void AddPermission(Permission permission)
    {
        if (!Permissions.Any(p => p.PermissionId == permission.Id))
        {
            Permissions.Add(new RolePermission { RoleId = this.Id, PermissionId = permission.Id });
        }
    }

    public void RemovePermission(Permission permission)
    {
        var rolePermission = Permissions.FirstOrDefault(p => p.PermissionId == permission.Id);
        if (rolePermission != null)
        {
            Permissions.Remove(rolePermission);
        }
    }
}