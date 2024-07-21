using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Interfaces;
using Shared.Common.ValueObjects;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Persistence;

namespace UserManagement.Infrastructure.Repositories
{
public class UserRepository : IUserRepository
    {
        private readonly UserManagementDbContext _context;
        private readonly IEncryptionService _encryptionService;

        public UserRepository(UserManagementDbContext context, IEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(Guid.Parse(id));
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var hashedEmail = EncryptedField.Create(email, _encryptionService).HashedValue;
            return await _context.Users.FirstOrDefaultAsync(u => u.EmailHash == hashedEmail);
        }

        public async Task<User?> GetByExternalProviderIdAsync(string provider, string id)
        {
            switch(provider.ToLowerInvariant())
            {
                case "google":
                    return await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == id);
                case "microsoft":
                    return await _context.Users.FirstOrDefaultAsync(u => u.MicrosoftId == id);
                default:
                    return null;
            }
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            var hashedRefreshToken = EncryptedField.Create(refreshToken, _encryptionService).HashedValue;
            return await _context.Users.FirstOrDefaultAsync(u => u.RefreshTokenHash == hashedRefreshToken);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}