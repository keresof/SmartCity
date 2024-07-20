using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Services;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly RsaSecurityKey _privateKey;
    private readonly RsaSecurityKey _publicKey;
    private readonly JsonWebTokenHandler _tokenHandler;
    private readonly ILogger<JwtTokenService> _logger;
    public RsaSecurityKey RsaSecurityKey => _publicKey;

    public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        RSA rsa;
        string jwtKeyFilepath = _configuration["JwtPrivateKeyFilepath"] ?? "private_key.pem";
        string solutionDir = GetSolutionDirectory();
        string fullPath = Path.Combine(solutionDir, jwtKeyFilepath);

        if (!string.IsNullOrEmpty(jwtKeyFilepath) && File.Exists(fullPath))
        {
            rsa = RSA.Create();
            _logger.LogDebug("Loading RSA keys from file");
            using var sr = new StreamReader(fullPath);
            var pem = sr.ReadToEnd();
            rsa.ImportFromPem(pem);
        }
        else
        {
            rsa = RSA.Create(2048);
            var filepath = _configuration["JwtPrivateKeyFilepath"] ?? "private_key.pem";
            logger.LogDebug("Generating new RSA keys for JWT token service");
            var pem = rsa.ExportRSAPrivateKeyPem();
            rsa.ImportFromPem(pem);
            string newKeyPath = Path.Combine(solutionDir, "private_key.pem");
            File.WriteAllText(newKeyPath, pem);
            logger.LogDebug($"New RSA private key saved to: {newKeyPath}");
        }

        _privateKey = new RsaSecurityKey(rsa);
        _publicKey = new RsaSecurityKey(rsa.ExportParameters(false));

        _tokenHandler = new JsonWebTokenHandler();
    }

    public string CreateAccessToken(User user)
    {
        var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("email_verified", user.IsEmailVerified.ToString().ToLower(), ClaimValueTypes.Boolean)
            };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["JwtIssuer"],
            Audience = _configuration["JwtAudience"],
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtExpiry"])),
            IssuedAt = DateTime.UtcNow,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256)
        };

        return _tokenHandler.CreateToken(tokenDescriptor);
    }

    public string CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<ClaimsPrincipal> ValidateToken(string token, bool validateLifetime = true)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = validateLifetime,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["JwtIssuer"],
            ValidAudience = _configuration["JwtAudience"],
            IssuerSigningKey = _publicKey,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var result = await _tokenHandler.ValidateTokenAsync(token, validationParameters);
            if (result.IsValid)
            {
                return new ClaimsPrincipal(result.ClaimsIdentity);
            }
        }
        catch (SecurityTokenException)
        {
            // Token validation failed
        }

        return null;
    }
    private string GetSolutionDirectory()
    {
        string currentDir = Directory.GetCurrentDirectory();
        while (!Directory.GetFiles(currentDir, "*.sln").Any())
        {
            DirectoryInfo parentDir = Directory.GetParent(currentDir);
            if (parentDir == null)
            {
                throw new DirectoryNotFoundException("Solution directory not found.");
            }
            currentDir = parentDir.FullName;
        }
        return currentDir;
    }
}

