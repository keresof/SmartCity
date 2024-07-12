using SmartCity.Core.Entities;

namespace SmartCity.Modules.UserManagemet.Domain
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        // Additional properties
    }
}