namespace CoursePlatform.Domain.Common;

/// <summary>
/// Envelope returned by repository and service search operations.
/// </summary>
/// <typeparam name="T">Type of the returned payload.</typeparam>
public sealed class BaseSearchResult<T>
{
    public int TotalCount { get; init; }

    public T? Results { get; init; }
}
