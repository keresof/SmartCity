
using MediatR;
using SmartCity.Core.Interfaces;
using SmartCity.Modules.UserManagemet.Domain;

namespace SmartCity.Modules.UserManagemet.Application.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(IRepository<User> userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                Email = request.Email
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return user.Id;
        }

        private string HashPassword(string password)
        {
            // Implement password hashing here
            return password;
        }

        public Task<int> Handle(RegisterUserCommand request)
        {
            throw new NotImplementedException();
        }
    }
}