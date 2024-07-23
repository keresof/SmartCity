using MediatR;
using Microsoft.Extensions.Configuration;
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
    private readonly IConfiguration _configuration;


    public RegisterUserCommandHandler(IUserRepository userRepository, ITokenService tokenService, IEncryptionService encryptionService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _encryptionService = encryptionService;
        _configuration = configuration;
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
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["RefreshTokenExpiry"] ?? "10")), _encryptionService);
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