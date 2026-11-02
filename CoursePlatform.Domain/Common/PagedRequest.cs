namespace CoursePlatform.Domain.Common;

/// <summary>
/// Paging information for search operations. Values are normalised so invalid
/// input never reaches the data store.
/// </summary>
public sealed record PagedRequest
{
    public const int DefaultPageSize = 20;

    public const int MaxPageSize = 200;

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    public int NormalizedPageNumber => PageNumber < 1 ? 1 : PageNumber;

    public int NormalizedPageSize => PageSize is < 1 or > MaxPageSize ? DefaultPageSize : PageSize;

    public int Skip => (NormalizedPageNumber - 1) * NormalizedPageSize;
}
