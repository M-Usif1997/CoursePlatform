using System.ComponentModel.DataAnnotations;

namespace CoursePlatform.Application.Features.Authentication.Dtos;

/// <summary>
/// Credentials submitted to <c>POST api/auth/login</c>.
/// </summary>
public sealed record LoginRequestDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not a valid address.")]
    [MaxLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [MaxLength(128)]
    public string Password { get; init; } = string.Empty;
}
