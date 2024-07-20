using MediatR;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using Shared.Common.Interfaces;
using Microsoft.Extensions.Configuration;
namespace UserManagement.Application.Commands.SendOTP;

public class SendOTPCommandHandler : IRequestHandler<SendOTPCommand, SendOTPResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IOTPService _otpService;
    private readonly IConfiguration _configuration;

    public SendOTPCommandHandler(IUserRepository userRepository, IOTPService otpService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _otpService = otpService;
        _configuration = configuration;
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

        var code = await _otpService.GenerateOTPAsync(user.Id.ToString(), request.Purpose, request.DeliveryMethod, TimeSpan.FromMinutes(Convert.ToUInt32(_configuration["OTP:ExpiryDuration"])));
        if (code is null)
        {
            return new SendOTPResult
            {
                Success = false,
                Message = "Failed to generate OTP"
            };
        }
        // TODO: Implement sending message to the broker to send the OTP code to the user.
        //      Notification Module will be responsible for sending the OTP code to the user.
        //      Once the message is queued, this function should return a success message.
        return new SendOTPResult
        {
            Success = true,
            Message = "OTP sent successfully (in queue)"
        };
    }
}