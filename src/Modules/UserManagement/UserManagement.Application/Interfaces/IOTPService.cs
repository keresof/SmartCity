using System.Threading.Tasks;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Interfaces
{
    public interface IOTPService
    {
        Task<string> GenerateOTPAsync(string userId, OTPPurpose purpose);
        Task<bool> ValidateOTPAsync(string userId, string code, OTPPurpose purpose);
        Task SendOTPAsync(string userId, string code, OTPDeliveryMethod method);
    }
}