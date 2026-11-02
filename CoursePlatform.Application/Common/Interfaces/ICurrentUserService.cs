namespace CoursePlatform.Application.Common.Interfaces;

/// <summary>
/// Identity of the caller of the current request.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? Email { get; }

    string? FullName { get; }

    bool IsAuthenticated { get; }
}
