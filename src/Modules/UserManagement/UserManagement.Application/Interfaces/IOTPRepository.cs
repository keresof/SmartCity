using System.Threading.Tasks;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces
{
    public interface IOTPRepository
    {
        Task<OTP> GetLatestOTPAsync(string userId, OTPPurpose purpose);
        Task AddAsync(OTP otp);
        Task UpdateAsync(OTP otp);
        Task SaveChangesAsync();
    }
}