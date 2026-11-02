using CoursePlatform.Application.Features.Authentication.Implementation;
using CoursePlatform.Application.Features.Authentication.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CoursePlatform.Application;

/// <summary>
/// Composition root for the application layer. Register every feature service
/// here; feature services use the "interface + internal implementation" pattern
/// shown by <see cref="IAuthService"/>.
/// </summary>
public static class ResolveDependencies
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Authentication (plumbing kept by the template)
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
