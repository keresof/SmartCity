using MediatR;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;


    public RegisterUserCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;

    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is not null)
        {
            return new RegisterUserResult
            {
                Success = false,
                Errors = ["Email already registered."]
            };
        }
        user = User.Create("", "", request.Email);
        user.SetPassword(request.Password);
        user.AddAuthenticationMethod(AuthenticationMethod.EmailPassword);
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var token = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        return new RegisterUserResult
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken
        };
    }
}