using StackExchange.Redis;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using System.Text.Json;

namespace UserManagement.Infrastructure.Services
{
    public class RedisOTPService : IOTPService
    {
        private readonly IConnectionMultiplexer _redis;
        private const string OTP_KEY_PREFIX = "otp:";

        public RedisOTPService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<OTP> GenerateOTPAsync(string userId, OTPPurpose purpose, OTPDeliveryMethod deliveryMethod, TimeSpan expiryDuration)
        {
            var db = _redis.GetDatabase();
            var code = GenerateOTPCode();
            var otp = OTP.Create(userId, code, DateTime.UtcNow.Add(expiryDuration), purpose, deliveryMethod);

            var key = GetOTPKey(userId, code);
            await db.StringSetAsync(key, JsonSerializer.Serialize(otp), expiryDuration);

            return otp;
        }

        public async Task<OTP?> GetOTPAsync(string userId, string code)
        {
            var db = _redis.GetDatabase();
            var key = GetOTPKey(userId, code);
            var otpJson = await db.StringGetAsync(key);

            return otpJson.HasValue 
                ? JsonSerializer.Deserialize<OTP>(otpJson)
                : null;
        }

        public async Task<bool> ValidateOTPAsync(string userId, string code, OTPPurpose purpose)
        {
            var otp = await GetOTPAsync(userId, code);
            return otp != null && otp.Purpose == purpose && otp.IsValid(DateTime.UtcNow);
        }

        public async Task MarkOTPAsUsedAsync(string userId, string code)
        {
            var db = _redis.GetDatabase();
            var key = GetOTPKey(userId, code);
            await db.KeyDeleteAsync(key);
        }

        public async Task<bool> IsOTPValidAsync(string userId, string code)
        {
            var otp = await GetOTPAsync(userId, code);
            return otp != null && otp.IsValid(DateTime.UtcNow);
        }

        private string GetOTPKey(string userId, string code) => $"{OTP_KEY_PREFIX}{userId}:{code}";

        private string GenerateOTPCode()
        {
            // generate a 6-digit random number
            return new Random().Next(100000, 999999).ToString();
        }
    }
}