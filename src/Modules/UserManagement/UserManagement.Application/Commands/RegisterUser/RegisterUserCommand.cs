using MediatR;

namespace UserManagement.Application.Commands.RegisterUser;
public class RegisterUserCommand : IRequest<RegisterUserResult>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterUserResult {
    public bool Success { get; set; }
    public string[] Errors { get; set; } = [];
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
}