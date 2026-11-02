using CoursePlatform.Application.Common.Interfaces;
using Serilog;

namespace CoursePlatform.Infrastructure.Services.Logging;

/// <summary>
/// Serilog implementation of <see cref="ILogService"/>.
/// </summary>
public sealed class SerilogLogService : ILogService
{
    public void LogInformation(string message, params object?[] args)
        => Log.Information(message, args);

    public void LogWarning(string message, params object?[] args)
        => Log.Warning(message, args);

    public void LogError(Exception exception, string message, params object?[] args)
        => Log.Error(exception, message, args);

    public void LogDebug(string message, params object?[] args)
        => Log.Debug(message, args);
}
