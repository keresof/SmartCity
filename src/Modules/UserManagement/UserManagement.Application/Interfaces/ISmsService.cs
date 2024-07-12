using System.Threading.Tasks;

namespace UserManagement.Application.Interfaces
{
    public interface ISmsService
    {
        Task SendOtpAsync(string phoneNumber, string otp);
    }
}