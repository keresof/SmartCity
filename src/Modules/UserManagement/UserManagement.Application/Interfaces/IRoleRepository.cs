using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
    // Add other permission-specific query methods
}