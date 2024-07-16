using MediatR;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
namespace UserManagement.Application.Commands.SendOTP;

public class SendOTPCommandHandler : IRequestHandler<SendOTPCommand, SendOTPResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IOTPRepository _otpRepository;

    public SendOTPCommandHandler(IUserRepository userRepository, IOTPRepository otpRepository)
    {
        _userRepository = userRepository;
        _otpRepository = otpRepository;
    }

    public async Task<SendOTPResult> Handle(SendOTPCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return new SendOTPResult
            {
                Success = false,
                Message = "User not found"
            };
        }

        var otp = OTP.Create
        (
            user.Id.ToString(),
            new Random().Next(1000, 9999).ToString(),
            DateTime.UtcNow.AddMinutes(5),
            request.Purpose,
            request.DeliveryMethod
        );

        await _otpRepository.AddAsync(otp);

        return new SendOTPResult
        {
            Success = true,
            Message = "OTP sent successfully"
        };
    }
}