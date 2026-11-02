using System.Security.Cryptography;
using CoursePlatform.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CoursePlatform.Infrastructure.Services.Security;

/// <summary>
/// PBKDF2-SHA256 password hashing with a per-password salt and a global pepper
/// from configuration.
/// </summary>
/// <remarks>
/// Stored format: <c>pbkdf2-sha256$iterations$base64(salt)$base64(key)</c>.
/// The iteration count is stored with the hash so it can be raised later without
/// invalidating existing passwords.
/// </remarks>
public sealed class PasswordHasherService : IPasswordHasherService
{
    private const string Algorithm = "pbkdf2-sha256";
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 210_000;
    private const char Separator = '$';

    private readonly string _pepper;

    public PasswordHasherService(IConfiguration configuration)
    {
        _pepper = configuration["Security:PasswordPepper"] is { Length: > 0 } pepper
            ? pepper
            : throw new InvalidOperationException("Security:PasswordPepper is not configured.");
    }

    public Task<string> HashPasswordAsync(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password is required.", nameof(password));
        }

        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var key = Rfc2898DeriveBytes.Pbkdf2(
            password + _pepper,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        var hash = string.Join(
            Separator,
            Algorithm,
            Iterations,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(key));

        return Task.FromResult(hash);
    }

    public Task<bool> VerifyPasswordAsync(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
        {
            return Task.FromResult(false);
        }

        var parts = hashedPassword.Split(Separator);

        if (parts.Length != 4 || !string.Equals(parts[0], Algorithm, StringComparison.Ordinal))
        {
            return Task.FromResult(false);
        }

        if (!int.TryParse(parts[1], out var iterations) || iterations <= 0)
        {
            return Task.FromResult(false);
        }

        byte[] salt;
        byte[] expectedKey;

        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expectedKey = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return Task.FromResult(false);
        }

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(
            password + _pepper,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expectedKey.Length);

        return Task.FromResult(CryptographicOperations.FixedTimeEquals(actualKey, expectedKey));
    }
}
