using CoursePlatform.Application.Common.Interfaces;

namespace CoursePlatform.Infrastructure.Services;

/// <summary>
/// System clock. Replace with <c>TimeProvider</c> in tests.
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
