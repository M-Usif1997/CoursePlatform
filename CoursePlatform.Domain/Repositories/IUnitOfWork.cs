using CoursePlatform.Domain.Common;

namespace CoursePlatform.Domain.Repositories;

/// <summary>
/// Coordinates repositories that share one persistence context and controls
/// when changes are committed.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    IBaseRepository<TEntity> GetRepository<TEntity>()
        where TEntity : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
