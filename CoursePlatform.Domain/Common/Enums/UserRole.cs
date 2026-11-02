namespace CoursePlatform.Domain.Common.Enums;

/// <summary>
/// Application roles. Add new roles here; they are emitted as role claims in
/// the issued JWT and enforced with [Authorize(Roles = ...)].
/// </summary>
public enum UserRole
{
    User = 1,
    Administrator = 2
}
