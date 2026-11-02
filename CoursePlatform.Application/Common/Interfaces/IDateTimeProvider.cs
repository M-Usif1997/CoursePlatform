namespace CoursePlatform.Application.Common.Interfaces;

/// <summary>
/// Clock abstraction so timestamps are testable.
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
