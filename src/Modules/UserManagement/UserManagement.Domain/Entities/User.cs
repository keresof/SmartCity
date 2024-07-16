using Shared.Common.Abstract;
using UserManagement.Domain.Enums;
namespace UserManagement.Domain.Entities
{
    public class User : AuditableEntity
    {
        public List<UserRole> Roles { get; private set; } = new List<UserRole>();
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string? GoogleId { get; private set; }
        public string? MicrosoftId { get; private set; }
        public string? FacebookId { get; private set; }
        public string? PhoneNumber { get; private set; }
        public Password PasswordHash { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }
        public string? ResetPasswordToken { get; private set; }
        public DateTime? ResetPasswordTokenExpiryTime { get; private set; }
        public bool IsEmailVerified { get; private set; }
        public bool IsPhoneNumberVerified { get; private set; }
        public bool IsActive { get; private set; } = true;
        public bool IsDeleted { get; private set; } = false;
        public List<AuthenticationMethod> AuthenticationMethods { get; private set; } = new List<AuthenticationMethod>();

        private User() { } // For EF Core

        public static User Create(string firstName, string lastName, string email)
        {
            return new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                IsActive = true,
                IsDeleted = false
            };
        }

        public void AddAuthenticationMethod(AuthenticationMethod method)
        {
            if (!AuthenticationMethods.Contains(method))
            {
                AuthenticationMethods.Add(method);
            }
        }

        public void SetPasswordHash(string passwordHash)
        {
            PasswordHash = Password.Create(passwordHash);
            AddAuthenticationMethod(AuthenticationMethod.EmailPassword);
        }

        public void SetOAuthId(string provider, string id)
        {
            switch (provider.ToLower())
            {
                case "google":
                    GoogleId = id;
                    AddAuthenticationMethod(AuthenticationMethod.Google);
                    break;
                case "microsoft":
                    MicrosoftId = id;
                    AddAuthenticationMethod(AuthenticationMethod.Microsoft);
                    break;
                case "facebook":
                    FacebookId = id;
                    AddAuthenticationMethod(AuthenticationMethod.Facebook);
                    break;
                default:
                    throw new ArgumentException("Invalid OAuth provider", nameof(provider));
            }
        }

        public void VerifyEmail()
        {
            IsEmailVerified = true;
        }

        public void SetRefreshToken(string token, DateTime expiryTime)
        {
            RefreshToken = token;
            RefreshTokenExpiryTime = expiryTime;
        }

        public void SetResetPasswordToken(string token, DateTime expiryTime)
        {
            ResetPasswordToken = token;
            ResetPasswordTokenExpiryTime = expiryTime;
        }

        public void ClearResetPasswordToken()
        {
            ResetPasswordToken = null;
            ResetPasswordTokenExpiryTime = null;
        }

        public void AddRole(Role role)
        {
            if (!Roles.Any(r => r.RoleId == role.Id))
            {
                Roles.Add(new UserRole { UserId = this.Id, RoleId = role.Id });
            }
        }

        public void RemoveRole(Role role)
        {
            var userRole = Roles.FirstOrDefault(r => r.RoleId == role.Id);
            if (userRole != null)
            {
                Roles.Remove(userRole);
            }
        }
    }
}