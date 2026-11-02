using CoursePlatform.Api.Extensions.Security;
using Microsoft.AspNetCore.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CoursePlatform.Api.Extensions;

/// <summary>
/// OpenAPI document generation (built into ASP.NET Core) plus the Swagger UI.
/// </summary>
public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

        return services;
    }

    public static WebApplication UseOpenApiDocumentation(this WebApplication app)
    {
        var title = app.Configuration["Swagger:Title"] ?? "CoursePlatform API";
        var version = app.Configuration["Swagger:Version"] ?? "v1";

        app.MapOpenApi();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/openapi/{version}.json", title);
            options.RoutePrefix = "swagger";
            options.DocumentTitle = $"{title} Documentation";
            options.DocExpansion(DocExpansion.None);
        });

        return app;
    }
}
