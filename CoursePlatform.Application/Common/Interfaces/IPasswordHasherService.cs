namespace CoursePlatform.Application.Common.Interfaces;

/// <summary>
/// Salted, peppered password hashing.
/// </summary>
public interface IPasswordHasherService
{
    Task<string> HashPasswordAsync(string password);

    Task<bool> VerifyPasswordAsync(string password, string hashedPassword);
}
