using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly RsaSecurityKey _privateKey;
        private readonly RsaSecurityKey _publicKey;
        private readonly JsonWebTokenHandler _tokenHandler;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            
            // Load RSA keys from configuration or generate new ones
            var rsa = RSA.Create();
            if (!string.IsNullOrEmpty(_configuration["JwtPrivateKey"]))
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(_configuration["JwtPrivateKey"]), out _);
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

        public async Task<ClaimsPrincipal> ValidateToken(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
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
    }
}