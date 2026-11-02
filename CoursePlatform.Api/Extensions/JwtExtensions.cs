using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CoursePlatform.Api.Extensions;

/// <summary>
/// JWT bearer authentication configuration.
/// </summary>
public static class JwtExtensions
{
    private const int MinimumSecretLength = 32;

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var secretKey = configuration["JwtSettings:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < MinimumSecretLength)
        {
            throw new InvalidOperationException(
                $"JwtSettings:SecretKey must be configured with at least {MinimumSecretLength} characters.");
        }

        var issuer = configuration["JwtSettings:Issuer"]
                     ?? throw new InvalidOperationException("JwtSettings:Issuer is not configured.");

        var audience = configuration["JwtSettings:Audience"]
                       ?? throw new InvalidOperationException("JwtSettings:Audience is not configured.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });

        services.AddAuthorization();

        return services;
    }
}
