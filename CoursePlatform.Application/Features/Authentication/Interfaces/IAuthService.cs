using CoursePlatform.Application.Features.Authentication.Dtos;

namespace CoursePlatform.Application.Features.Authentication.Interfaces;

/// <summary>
/// Authentication use cases.
/// </summary>
public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
}
