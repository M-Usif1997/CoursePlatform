using CoursePlatform.Application.Common.Interfaces;
using CoursePlatform.Domain.Common.Enums;
using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CoursePlatform.Infrastructure.Persistence;

/// <summary>
/// Applies migrations and seeds reference data at startup when the matching
/// configuration flags are enabled.
/// </summary>
public interface IDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

/// <inheritdoc />
public sealed class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IConfiguration _configuration;
    private readonly ILogService _logService;

    public DbInitializer(
        ApplicationDbContext context,
        IPasswordHasherService passwordHasherService,
        IConfiguration configuration,
        ILogService logService)
    {
        _context = context;
        _passwordHasherService = passwordHasherService;
        _configuration = configuration;
        _logService = logService;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_configuration.GetValue("Database:ApplyMigrationsOnStartup", false))
        {
            await _context.Database.MigrateAsync(cancellationToken);
            _logService.LogInformation("Database migrations applied.");
        }

        if (!_configuration.GetValue("Database:SeedOnStartup", false))
        {
            return;
        }

        var email = _configuration["Seed:AdminEmail"];
        var password = _configuration["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logService.LogWarning("Seeding is enabled but Seed:AdminEmail / Seed:AdminPassword are not configured.");
            return;
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(user => user.Email == normalizedEmail, cancellationToken))
        {
            return;
        }

        _context.Users.Add(new User
        {
            Email = normalizedEmail,
            FullName = _configuration["Seed:AdminFullName"] ?? "System Administrator",
            PasswordHash = await _passwordHasherService.HashPasswordAsync(password),
            Role = UserRole.Administrator
        });

        await _context.SaveChangesAsync(cancellationToken);
        _logService.LogInformation("Seeded administrator account {Email}.", normalizedEmail);
    }
}
