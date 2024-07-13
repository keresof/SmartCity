using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Infrastructure.Persistence;

namespace UserManagement.Infrastructure.Repositories
{
    public class OTPRepository : IOTPRepository
    {
        private readonly UserManagementDbContext _context;

        public OTPRepository(UserManagementDbContext context)
        {
            _context = context;
        }

        public async Task<OTP> GetLatestOTPAsync(string userId, OTPPurpose purpose)
        {
            return await _context.OTPs
                .Where(o => o.UserId == userId && o.Purpose == purpose)
                .OrderByDescending(o => o.ExpiryTime)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(OTP otp)
        {
            await _context.OTPs.AddAsync(otp);
        }

        public async Task UpdateAsync(OTP otp)
        {
            _context.Entry(otp).State = EntityState.Modified;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}