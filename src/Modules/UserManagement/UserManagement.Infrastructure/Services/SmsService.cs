using System.Threading.Tasks;
using UserManagement.Application.Interfaces;

namespace UserManagement.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        public async Task SendOtpAsync(string phoneNumber, string otp)
        {
            // Implement SMS sending logic here
            await Task.CompletedTask;
        }
    }
}