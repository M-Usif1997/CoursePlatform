using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.Api.Extensions;

/// <summary>
/// Makes model validation failures use the same error envelope as the
/// exception middleware, so clients parse one shape only.
/// </summary>
public static class ApiBehaviorExtensions
{
    public static IServiceCollection AddModelValidationEnvelope(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(entry => entry.Value is { Errors.Count: > 0 })
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors
                            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "Invalid value."
                                : error.ErrorMessage)
                            .ToArray());

                return new BadRequestObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = "Validation failed.",
                    errors,
                    traceId = context.HttpContext.TraceIdentifier
                });
            };
        });

        return services;
    }
}
