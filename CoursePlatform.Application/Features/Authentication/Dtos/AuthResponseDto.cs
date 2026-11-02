namespace CoursePlatform.Application.Features.Authentication.Dtos;

/// <summary>
/// Result of a successful sign in or registration.
/// </summary>
public sealed record AuthResponseDto
{
    public bool IsSuccess { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? AccessToken { get; init; }

    public DateTime? AccessTokenExpiresAtUtc { get; init; }

    public Guid UserId { get; init; }

    public string? Email { get; init; }

    public string? FullName { get; init; }

    public string? Role { get; init; }
}
