using System.ComponentModel.DataAnnotations;

namespace CoursePlatform.Application.Features.Authentication.Dtos;

/// <summary>
/// Payload for <c>POST api/auth/register</c>. Accounts created here always get
/// the <c>User</c> role.
/// </summary>
public sealed record RegisterRequestDto
{
    [Required(ErrorMessage = "Full name is required.")]
    [MaxLength(200)]
    public string FullName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not a valid address.")]
    [MaxLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [MaxLength(128)]
    public string Password { get; init; } = string.Empty;
}
