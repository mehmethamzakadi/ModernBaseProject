using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModernBaseProject.Core.Constants;
using ModernBaseProject.Core.Domain.Entities;

namespace ModernBaseProject.Infrastructure.Authentication;

public class JwtProvider
{
    private readonly IConfiguration _configuration;

    public JwtProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user, IEnumerable<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtClaims.Permissions, string.Join(",", permissions))
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[ConfigurationKeys.Jwt.Key]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration[ConfigurationKeys.Jwt.Issuer],
            audience: _configuration[ConfigurationKeys.Jwt.Audience],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(JwtDefaults.AccessTokenExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
