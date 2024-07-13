using Shared.Common.Abstract;

using UserManagement.Domain.Enums;


namespace UserManagement.Domain.Entities
{
    public class OTP : BaseEntity
    {
        public string UserId { get; private set; }
        public string Code { get; private set; }
        public DateTime ExpiryTime { get; private set; }
        public OTPPurpose Purpose { get; private set; }
        public bool IsUsed { get; private set; }

        private OTP() { } // For EF Core

        public static OTP Create(string userId, string code, DateTime expiryTime, OTPPurpose purpose)
        {
            return new OTP
            {
                UserId = userId,
                Code = code,
                ExpiryTime = expiryTime,
                Purpose = purpose,
                IsUsed = false
            };
        }

        public void MarkAsUsed()
        {
            IsUsed = true;
        }

        public bool IsValid(DateTime currentTime)
        {
            return !IsUsed && currentTime <= ExpiryTime;
        }
    }
}