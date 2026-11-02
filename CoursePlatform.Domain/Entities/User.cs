using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Common.Enums;

namespace CoursePlatform.Domain.Entities;

/// <summary>
/// Application user. Replace or extend this entity when a feature needs more
/// profile data, and add the new properties to <c>UserConfiguration</c>.
/// </summary>
public sealed class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Salted PBKDF2 hash produced by <c>IPasswordHasherService</c>.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;

    public bool IsActive { get; set; } = true;
}
