using MediatR;
using Shared.Common.Interfaces;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IEncryptionService _encryptionService;


    public RegisterUserCommandHandler(IUserRepository userRepository, ITokenService tokenService, IEncryptionService encryptionService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _encryptionService = encryptionService;
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
        user = User.Create("", "", request.Email, _encryptionService);
        user.SetPassword(request.Password);
        user.AddAuthenticationMethod(AuthenticationMethod.EmailPassword);
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        user.CreatedBy = user.Id.ToString();
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        var token = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return new RegisterUserResult
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken
        };
    }
}