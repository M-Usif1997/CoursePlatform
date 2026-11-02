using System.Linq.Expressions;
using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Repositories;

/// <summary>
/// Persistence-agnostic CRUD and query contract for an aggregate.
/// </summary>
public interface IBaseRepository<TEntity>
    where TEntity : BaseEntity
{
    /// <summary>
    /// Composable query entry point for feature specific reads.
    /// </summary>
    IQueryable<TEntity> Query(bool asNoTracking = true);

    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TEntity?> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<BaseSearchResult<IReadOnlyList<TEntity>>> SearchAsync(
        PagedRequest request,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}
