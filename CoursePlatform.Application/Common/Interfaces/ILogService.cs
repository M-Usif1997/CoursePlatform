namespace CoursePlatform.Application.Common.Interfaces;

/// <summary>
/// Logging abstraction so the application layer does not depend on a logging
/// framework.
/// </summary>
public interface ILogService
{
    void LogInformation(string message, params object?[] args);

    void LogWarning(string message, params object?[] args);

    void LogError(Exception exception, string message, params object?[] args);

    void LogDebug(string message, params object?[] args);
}
