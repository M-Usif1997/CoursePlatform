using System.Text;
using CoursePlatform.Api.Extensions;
using CoursePlatform.Api.Middlewares;
using CoursePlatform.Application;
using CoursePlatform.Infrastructure;
using CoursePlatform.Infrastructure.Persistence;
using CoursePlatform.Infrastructure.Services.Logging;
using Serilog;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOpenApiDocumentation();

builder.Services.AddControllers();
builder.Services.AddModelValidationEnvelope();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseOpenApiDocumentation();
app.MapControllers();

if (app.Configuration.GetValue("Database:ApplyMigrationsOnStartup", false) ||
    app.Configuration.GetValue("Database:SeedOnStartup", false))
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<IDbInitializer>().InitializeAsync();
}

app.Run();
