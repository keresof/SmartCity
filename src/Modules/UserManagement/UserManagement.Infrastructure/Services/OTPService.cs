using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Services
{
    public class OTPService : IOTPService
    {
        private readonly IOTPRepository _otpRepository;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IUserRepository _userRepository;

        public OTPService(IOTPRepository otpRepository, IEmailService emailService, ISmsService smsService, IUserRepository userRepository)
        {
            _otpRepository = otpRepository;
            _emailService = emailService;
            _smsService = smsService;
            _userRepository = userRepository;
        }

        public async Task<string> GenerateOTPAsync(string userId, OTPPurpose purpose)
        {
            string code = GenerateOTPCode();
            var otp = OTP.Create(userId, code, DateTime.UtcNow.AddMinutes(5), purpose);
            await _otpRepository.AddAsync(otp);
            await _otpRepository.SaveChangesAsync();
            return code;
        }

        public async Task<bool> ValidateOTPAsync(string userId, string code, OTPPurpose purpose)
        {
            var otp = await _otpRepository.GetLatestOTPAsync(userId, purpose);
            if (otp == null || !otp.IsValid(DateTime.UtcNow) || otp.Code != code)
            {
                return false;
            }

            otp.MarkAsUsed();
            await _otpRepository.UpdateAsync(otp);
            await _otpRepository.SaveChangesAsync();
            return true;
        }

        public async Task SendOTPAsync(string userId, string code, OTPDeliveryMethod method)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(userId));
            }

            switch (method)
            {
                case OTPDeliveryMethod.Email:
                    await _emailService.SendOtpAsync(user.Email, code);
                    break;
                case OTPDeliveryMethod.Sms:
                    if (string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        throw new InvalidOperationException("User does not have a phone number");
                    }
                    await _smsService.SendOtpAsync(user.PhoneNumber, code);
                    break;
                default:
                    throw new ArgumentException("Invalid OTP delivery method", nameof(method));
            }
        }

        private string GenerateOTPCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString("D6");
        }
    }

}