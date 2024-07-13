using System.Threading.Tasks;

namespace UserManagement.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpAsync(string email, string otp);
    }
}