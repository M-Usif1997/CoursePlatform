namespace CoursePlatform.Application.Common.Models;

/// <summary>
/// Issued access token together with its expiry.
/// </summary>
public sealed record JwtTokenResultDto(string AccessToken, DateTime ExpiresAtUtc);
