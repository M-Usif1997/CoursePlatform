using CoursePlatform.Application.Common.Interfaces;
using CoursePlatform.Application.Features.Authentication.Dtos;
using CoursePlatform.Application.Features.Authentication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.Api.Controllers.Common;

/// <summary>
/// Sign in, registration and caller identity endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(IAuthService authService, ICurrentUserService currentUserService)
    {
        _authService = authService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Exchanges credentials for a JWT access token.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
        => Ok(await _authService.LoginAsync(request, cancellationToken));

    /// <summary>
    /// Creates a new account with the <c>User</c> role.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
        => Ok(await _authService.RegisterAsync(request, cancellationToken));

    /// <summary>
    /// Returns the identity resolved from the bearer token; useful as a smoke
    /// test that JWT validation is wired correctly.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
        => Ok(new
        {
            userId = _currentUserService.UserId,
            email = _currentUserService.Email,
            fullName = _currentUserService.FullName,
            isAuthenticated = _currentUserService.IsAuthenticated
        });
}
