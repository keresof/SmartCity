namespace UserManagement.Application.DTOs;

public class OAuthUserInfo
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? Name { get; set; }
    public string? Picture { get; set; }
}