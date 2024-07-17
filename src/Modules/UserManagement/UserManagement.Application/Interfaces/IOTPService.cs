using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Interfaces
{
    public interface IOTPService
    {
        Task<OTP> GenerateOTPAsync(string userId, OTPPurpose purpose, OTPDeliveryMethod deliveryMethod, TimeSpan expiryDuration);
        
        Task<OTP?> GetOTPAsync(string userId, string code);
        
        Task<bool> ValidateOTPAsync(string userId, string code, OTPPurpose purpose);
        
        Task MarkOTPAsUsedAsync(string userId, string code);
        
        Task<bool> IsOTPValidAsync(string userId, string code);
    }
}