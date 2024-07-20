using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces;

public interface IPermissionRepository
{
    Task<Permission?> GetByNameAsync(string name);
    // Add other permission-specific query methods
}