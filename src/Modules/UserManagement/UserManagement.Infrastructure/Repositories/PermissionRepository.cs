using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Persistence;

namespace UserManagement.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly UserManagementDbContext _dbContext;

    public PermissionRepository(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Permission?> GetByNameAsync(string name)
    {
        return await _dbContext.Permissions.FirstOrDefaultAsync(r => r.Name == name);
    }

}