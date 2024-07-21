using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Persistence;

namespace UserManagement.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManagementDbContext _context;

        public UserRepository(UserManagementDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(Guid.Parse(id));
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task<User?> GetByExternalProviderIdAsync(string provider, string id)
        {
            switch (provider.ToLower())
            {
                case "google":
                    return _context.Users.FirstOrDefaultAsync(u => u.GoogleId == id);
                case "microsoft":
                    return _context.Users.FirstOrDefaultAsync(u => u.MicrosoftId == id);
                case "facebook":
                    return _context.Users.FirstOrDefaultAsync(u => u.FacebookId == id);
                default:
                    return Task.FromResult<User?>(null);
            }
        }

        public Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
    }
}