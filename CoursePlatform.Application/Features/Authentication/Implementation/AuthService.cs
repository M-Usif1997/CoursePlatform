using System.Security.Claims;
using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Common.Interfaces;
using CoursePlatform.Application.Features.Authentication.Dtos;
using CoursePlatform.Application.Features.Authentication.Interfaces;
using CoursePlatform.Domain.Common.Enums;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Repositories;

namespace CoursePlatform.Application.Features.Authentication.Implementation;

/// <summary>
/// Validates credentials against the <see cref="User"/> aggregate and issues a
/// JWT access token.
/// </summary>
internal sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ITokenProvider _tokenProvider;
    private readonly ILogService _logService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasherService passwordHasherService,
        ITokenProvider tokenProvider,
        ILogService logService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasherService = passwordHasherService;
        _tokenProvider = tokenProvider;
        _logService = logService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = NormalizeEmail(request.Email);
        var users = _unitOfWork.GetRepository<User>();

        var user = await users.GetFirstOrDefaultAsync(
            u => u.Email == email && u.IsActive,
            asNoTracking: true,
            cancellationToken);

        if (user is null ||
            !await _passwordHasherService.VerifyPasswordAsync(request.Password, user.PasswordHash))
        {
            _logService.LogWarning("Failed sign in attempt for {Email}.", email);
            throw new UnauthenticatedException();
        }

        _logService.LogInformation("User {UserId} signed in.", user.Id);

        return CreateAuthResponse(user, "Authentication succeeded.");
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = NormalizeEmail(request.Email);
        var users = _unitOfWork.GetRepository<User>();

        if (await users.ExistsAsync(u => u.Email == email, cancellationToken))
        {
            throw new ConflictException("An account with this email already exists.");
        }

        var user = new User
        {
            Email = email,
            FullName = request.FullName.Trim(),
            PasswordHash = await _passwordHasherService.HashPasswordAsync(request.Password),
            Role = UserRole.User
        };

        await users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logService.LogInformation("Registered new user {UserId}.", user.Id);

        return CreateAuthResponse(user, "Account created successfully.");
    }

    private AuthResponseDto CreateAuthResponse(User user, string message)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var token = _tokenProvider.GenerateAccessToken(claims);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = message,
            AccessToken = token.AccessToken,
            AccessTokenExpiresAtUtc = token.ExpiresAtUtc,
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString()
        };
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
