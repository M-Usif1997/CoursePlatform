using CoursePlatform.Domain.Common;

namespace CoursePlatform.Application.Services.Base;

/// <summary>
/// Generic CRUD contract over an aggregate, used by
/// <c>BaseController&lt;TEntity, TGetDto, TPostDto&gt;</c>.
/// </summary>
public interface IGenericBaseService<TEntity, TGetDto, TPostDto>
    where TEntity : BaseEntity
    where TGetDto : class
    where TPostDto : class
{
    Task<TGetDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TGetDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<BaseSearchResult<IReadOnlyList<TGetDto>>> SearchAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<TGetDto> CreateAsync(TPostDto postDto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TGetDto>> CreateRangeAsync(
        IEnumerable<TPostDto> postDtos,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Guid id, TPostDto postDto, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
