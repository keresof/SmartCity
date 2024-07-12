using SmartCity.Core.Interfaces;

namespace SmartCity.Modules.UserManagemet.Domain
{
    public class RegisterUserCommand : IRequest<int>, MediatR.IRequest<int>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
