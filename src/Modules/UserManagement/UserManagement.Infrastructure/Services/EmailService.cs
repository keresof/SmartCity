using System.Threading.Tasks;
using UserManagement.Application.Interfaces;

namespace UserManagement.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendOtpAsync(string email, string otp)
        {
            // Implement email sending logic here
            await Task.CompletedTask;
        }
    }
}
