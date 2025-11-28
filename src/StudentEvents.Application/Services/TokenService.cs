using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using StudentEvents.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace StudentEvents.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var keyString = _configuration.GetValue<string>("JwtSettings:Key") ?? "please-change-this-secret-key";

            // Ensure key is at least256 bits (32 bytes) required by HMAC-SHA256
            byte[] keyBytes = Encoding.UTF8.GetBytes(keyString);
            if (keyBytes.Length < 32)
            {
                // Derive a256-bit key from the configured string using SHA-256
                keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(keyString));
            }

            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                 new Claim(JwtRegisteredClaimNames.Email, user.Email),
                 new Claim("name", user.DisplayName)
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            // read expire minutes from configuration, default to120
            var expireMinutes = _configuration.GetValue<int?>("JwtSettings:ExpireMinutes") ?? 120;

            var token = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("JwtSettings:Issuer"),
            audience: _configuration.GetValue<string>("JwtSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
