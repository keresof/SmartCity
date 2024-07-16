using UserManagement.Application.Commands.CreateRole;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Interfaces;

public interface IRoleService
{
    Task<RoleDto> CreateRoleAsync(CreateRoleCommand command);
    Task<RoleDto> GetRoleByIdAsync(Guid id);
    Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);
    Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId);
}