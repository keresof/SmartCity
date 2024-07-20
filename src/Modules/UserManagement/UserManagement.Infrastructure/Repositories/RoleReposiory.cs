using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Persistence;

namespace UserManagement.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly UserManagementDbContext _dbContext;

    public RoleRepository(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }

}