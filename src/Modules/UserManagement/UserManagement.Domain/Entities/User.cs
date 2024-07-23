using Shared.Common.Abstract;
using Shared.Common.Interfaces;
using Shared.Common.ValueObjects;
using UserManagement.Domain.Enums;
using UserManagement.Domain.ValueObjects;
namespace UserManagement.Domain.Entities
{
    public class User : AuditableEntity
    {
        public List<UserRole> Roles { get; private set; } = new List<UserRole>();
        public EncryptedField FirstName { get; private set; }
        public string FirstNameHash { get; private set; }
        public EncryptedField LastName { get; private set; }
        public string LastNameHash { get; private set; }
        public EncryptedField Email { get; private set; }
        public string EmailHash { get; private set; }
        public string? GoogleId { get; private set; }
        public string? MicrosoftId { get; private set; }
        public string? FacebookId { get; private set; }
        public EncryptedField? PhoneNumber { get; private set; }
        public string? PhoneNumberHash { get; private set; }
        public Password? PasswordHash { get; private set; }
        public EncryptedField? RefreshToken { get; private set; }
        public string? RefreshTokenHash { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }
        public EncryptedField? ResetPasswordToken { get; private set; }
        public string? ResetPasswordTokenHash { get; private set; }
        public DateTime? ResetPasswordTokenExpiryTime { get; private set; }
        public bool IsEmailVerified { get; private set; }
        public bool IsPhoneNumberVerified { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
        public List<AuthenticationMethod> AuthenticationMethods { get; private set; } = new List<AuthenticationMethod>();

        public static User Create(string firstName, string lastName, string email, IEncryptionService encryptionService)
        {
            var user = new User{
                IsActive = true,
                IsDeleted = false,
                Created = DateTime.UtcNow,
            };
            user.SetFirstName(firstName ?? " ", encryptionService);
            user.SetLastName(lastName ?? " ", encryptionService);
            user.SetEmail(email, encryptionService);
            return user;
        }

        public void AddAuthenticationMethod(AuthenticationMethod method)
        {
            if (!AuthenticationMethods.Contains(method))
            {
                AuthenticationMethods.Add(method);
            }
        }

        public void SetPassword(string plainTextPassword)
        {
            PasswordHash = Password.Create(plainTextPassword);
            AddAuthenticationMethod(AuthenticationMethod.EmailPassword);
        }

        public string? GetOAuthId(string provider)
        {
            return provider.ToLower() switch
            {
                "google" => GoogleId,
                "microsoft" => MicrosoftId,
                "facebook" => FacebookId,
                _ => throw new ArgumentException("Invalid OAuth provider", nameof(provider))
            };
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

        public bool HasOAuthId(string provider)
        {
            return provider.ToLower() switch
            {
                "google" => GoogleId != null,
                "microsoft" => MicrosoftId != null,
                "facebook" => FacebookId != null,
                _ => false
            };
        }

        public bool HasPassword()
        {
            return PasswordHash != null;
        }

        public void VerifyEmail()
        {
            IsEmailVerified = true;
        }

        public void SetRefreshToken(string? token, DateTime expiryTime, IEncryptionService encryptionService)
        {
            RefreshTokenExpiryTime = expiryTime;
            if(token == null)
            {
                RefreshToken = null;
                RefreshTokenHash = null;
                return;
            }
            RefreshToken = EncryptedField.Create(token, encryptionService);
            RefreshTokenHash = RefreshToken.HashedValue;
        }

        public void SetResetPasswordToken(string token, DateTime expiryTime, IEncryptionService encryptionService)
        {
            ResetPasswordToken = EncryptedField.Create(token, encryptionService);
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
        public void SetFirstName(string firstName, IEncryptionService encryptionService)
        {
            var encryptedFirstName = EncryptedField.Create(firstName, encryptionService);
            FirstNameHash = encryptedFirstName.HashedValue;
            FirstName = encryptedFirstName;
        }

        public void SetLastName(string lastName, IEncryptionService encryptionService)
        {
            var encryptedLastName = EncryptedField.Create(lastName, encryptionService);
            LastNameHash = encryptedLastName.HashedValue;
            LastName = encryptedLastName;
        }

        public void SetPhoneNumber(string phoneNumber, IEncryptionService encryptionService)
        {
            var encryptedPhoneNumber = EncryptedField.Create(phoneNumber, encryptionService);
            PhoneNumberHash = encryptedPhoneNumber.HashedValue;
            PhoneNumber = encryptedPhoneNumber;
        }

        public void SetEmail(string email, IEncryptionService encryptionService)
        {
            var encryptedEmail = EncryptedField.Create(email, encryptionService);
            EmailHash = encryptedEmail.HashedValue;
            Email = encryptedEmail;
        }

        public void VerifyPhoneNumber()
        {
            IsPhoneNumberVerified = true;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Delete()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }
    }
}