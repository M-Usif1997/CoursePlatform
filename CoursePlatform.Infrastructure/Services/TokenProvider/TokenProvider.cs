using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoursePlatform.Application.Common.Interfaces;
using CoursePlatform.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoursePlatform.Infrastructure.Services.TokenProvider;

/// <summary>
/// Issues HMAC-SHA256 signed JWTs using the <c>JwtSettings</c> configuration
/// section.
/// </summary>
public sealed class TokenProvider : ITokenProvider
{
    private const int MinimumSecretLength = 32;

    private readonly IConfiguration _configuration;

    public TokenProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public JwtTokenResultDto GenerateAccessToken(IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);

        var secretKey = _configuration["JwtSettings:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < MinimumSecretLength)
        {
            throw new InvalidOperationException(
                $"JwtSettings:SecretKey must be configured with at least {MinimumSecretLength} characters.");
        }

        var issuer = _configuration["JwtSettings:Issuer"]
                     ?? throw new InvalidOperationException("JwtSettings:Issuer is not configured.");

        var audience = _configuration["JwtSettings:Audience"]
                       ?? throw new InvalidOperationException("JwtSettings:Audience is not configured.");

        var durationInMinutes = _configuration.GetValue("JwtSettings:DurationInMinutes", 60);

        var issuedAtUtc = DateTime.UtcNow;
        var expiresAtUtc = issuedAtUtc.AddMinutes(durationInMinutes);

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: issuedAtUtc,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials);

        return new JwtTokenResultDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAtUtc);
    }
}
