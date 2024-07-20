using UserManagement.Application.DTOs;
using UserManagement.Application.Commands.RegisterUser;

namespace UserManagement.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> RegisterUserAsync(RegisterUserCommand command);
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task AssignRoleToUserAsync(Guid userId, Guid roleId);
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);
    Task<bool> VerifyEmailAsync(string email, string token);
    Task RequestPasswordResetAsync(string email);
    Task ResetPasswordAsync(string email, string token, string newPassword);
}