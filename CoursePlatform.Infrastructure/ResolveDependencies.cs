using CoursePlatform.Application.Common.Interfaces;
using CoursePlatform.Domain.Repositories;
using CoursePlatform.Infrastructure.Persistence;
using CoursePlatform.Infrastructure.Persistence.Interceptors;
using CoursePlatform.Infrastructure.Persistence.Repositories;
using CoursePlatform.Infrastructure.Services;
using CoursePlatform.Infrastructure.Services.Logging;
using CoursePlatform.Infrastructure.Services.Security;
using CoursePlatform.Infrastructure.Services.TokenProvider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoursePlatform.Infrastructure;

/// <summary>
/// Composition root for the infrastructure layer: database, repositories and
/// concrete implementations of the application abstractions.
/// </summary>
public static class ResolveDependencies
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' is not configured.");

        services.AddHttpContextAccessor();

        // Cross-cutting services
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<ILogService, SerilogLogService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<ITokenProvider, TokenProvider>();

        // Persistence
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(
                connectionString,
                sqlServer => sqlServer.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            options.AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
        });

        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDbInitializer, DbInitializer>();

        return services;
    }
}
