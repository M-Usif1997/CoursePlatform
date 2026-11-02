using System.Net;
using CoursePlatform.Application.Common.Exceptions;

namespace CoursePlatform.Api.Middlewares;

/// <summary>
/// Converts application exceptions into the shared JSON error envelope
/// (<c>status</c>, <c>message</c>, <c>errors</c>, <c>traceId</c>).
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogInformation("Request {Method} {Path} was cancelled by the client.",
                context.Request.Method, context.Request.Path);
        }
        catch (Exception exception)
        {
            var (status, message, errors) = Map(exception);

            if (status == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception for {Method} {Path}.",
                    context.Request.Method, context.Request.Path);
            }
            else
            {
                // Expected outcome: log without a stack trace to keep error logs meaningful.
                _logger.LogWarning("Request {Method} {Path} failed with {StatusCode}: {Message}",
                    context.Request.Method, context.Request.Path, (int)status, message);
            }

            await WriteResponseAsync(context, status, message, errors);
        }
    }

    private static async Task WriteResponseAsync(
        HttpContext context,
        HttpStatusCode status,
        string message,
        object? errors)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            status = (int)status,
            message,
            errors,
            traceId = context.TraceIdentifier
        });
    }

    private (HttpStatusCode Status, string Message, object? Errors) Map(Exception exception) => exception switch
    {
        BadRequestException e => (HttpStatusCode.BadRequest, e.Message, e.Errors),
        ConflictException e => (HttpStatusCode.Conflict, e.Message, null),
        ForbiddenAccessException e => (HttpStatusCode.Forbidden, e.Message, null),
        UnauthenticatedException e => (HttpStatusCode.Unauthorized, e.Message, null),
        UnauthorizedAccessException e => (HttpStatusCode.Unauthorized, e.Message, null),
        EntityNotFoundException e => (HttpStatusCode.NotFound, e.Message, null),
        NoDataFoundException e => (HttpStatusCode.NotFound, e.Message, null),
        UserFriendlyException e => (HttpStatusCode.BadRequest, e.Message, null),
        ArgumentException e => (HttpStatusCode.BadRequest, e.Message, null),
        KeyNotFoundException e => (HttpStatusCode.BadRequest, e.Message, null),
        InvalidOperationException e => (HttpStatusCode.BadRequest, e.Message, null),
        _ => (
            HttpStatusCode.InternalServerError,
            "An unexpected error occurred.",
            _environment.IsDevelopment() ? new { exception.Message, exception.StackTrace } : null)
    };
}
