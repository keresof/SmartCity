using System.Security.Claims;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(User user);
    string CreateRefreshToken();
    Task<ClaimsPrincipal> ValidateToken(string token);
}
