using Shared.Common.Abstract;

namespace UserManagement.Domain.Entities
{
    public class User : AuditableEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? GoogleID { get; set; }
        public string? MicrosoftID { get; set; }
        public string? FacebookID { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? Salt { get; set; }
        public string? RefreshToken { get; set; }
        public string? ResetPasswordToken { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}