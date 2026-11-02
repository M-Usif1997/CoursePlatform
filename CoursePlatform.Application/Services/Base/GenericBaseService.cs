using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Domain.Common;
using CoursePlatform.Domain.Repositories;

namespace CoursePlatform.Application.Services.Base;

/// <summary>
/// Generic CRUD implementation. A feature service derives from this type and
/// supplies the three mapping members; everything else is inherited.
/// </summary>
/// <example>
/// <code>
/// public sealed class OrderService : GenericBaseService&lt;Order, OrderDto, PostOrderDto&gt;, IOrderService
/// {
///     public OrderService(IUnitOfWork unitOfWork) : base(unitOfWork) { }
///
///     protected override OrderDto MapToDto(Order entity) => new() { Id = entity.Id, ... };
///     protected override Order MapToEntity(PostOrderDto dto) => new() { ... };
///     protected override void ApplyDto(PostOrderDto dto, Order entity) => entity.Name = dto.Name;
/// }
/// </code>
/// </example>
public abstract class GenericBaseService<TEntity, TGetDto, TPostDto> : IGenericBaseService<TEntity, TGetDto, TPostDto>
    where TEntity : BaseEntity
    where TGetDto : class
    where TPostDto : class
{
    protected GenericBaseService(IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);

        UnitOfWork = unitOfWork;
        Repository = unitOfWork.GetRepository<TEntity>();
    }

    protected IUnitOfWork UnitOfWork { get; }

    protected IBaseRepository<TEntity> Repository { get; }

    protected abstract TGetDto MapToDto(TEntity entity);

    protected abstract TEntity MapToEntity(TPostDto postDto);

    protected abstract void ApplyDto(TPostDto postDto, TEntity entity);

    /// <summary>
    /// Hook for feature rules that must hold before a write. Throw
    /// <see cref="BadRequestException"/> or <see cref="ConflictException"/>.
    /// </summary>
    protected virtual Task ValidateAsync(TPostDto postDto, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public virtual async Task<TGetDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken)
                     ?? throw new EntityNotFoundException($"{typeof(TEntity).Name} with id '{id}' was not found.");

        return MapToDto(entity);
    }

    public virtual async Task<IReadOnlyList<TGetDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetAllAsync(cancellationToken);

        return entities.Select(MapToDto).ToList();
    }

    public virtual async Task<BaseSearchResult<IReadOnlyList<TGetDto>>> SearchAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await Repository.SearchAsync(request, cancellationToken: cancellationToken);

        return new BaseSearchResult<IReadOnlyList<TGetDto>>
        {
            TotalCount = result.TotalCount,
            Results = result.Results?.Select(MapToDto).ToList()
        };
    }

    public virtual async Task<TGetDto> CreateAsync(TPostDto postDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(postDto);

        await ValidateAsync(postDto, cancellationToken);

        var entity = MapToEntity(postDto);
        await Repository.AddAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public virtual async Task<IReadOnlyList<TGetDto>> CreateRangeAsync(
        IEnumerable<TPostDto> postDtos,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(postDtos);

        var dtos = postDtos.ToList();
        if (dtos.Count == 0)
        {
            throw new BadRequestException("At least one item is required.");
        }

        var entities = new List<TEntity>(dtos.Count);
        foreach (var dto in dtos)
        {
            await ValidateAsync(dto, cancellationToken);
            entities.Add(MapToEntity(dto));
        }

        await Repository.AddRangeAsync(entities, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return entities.Select(MapToDto).ToList();
    }

    public virtual async Task UpdateAsync(Guid id, TPostDto postDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(postDto);

        await ValidateAsync(postDto, cancellationToken);

        var entity = await Repository.GetByIdAsync(id, cancellationToken)
                     ?? throw new EntityNotFoundException($"{typeof(TEntity).Name} with id '{id}' was not found.");

        ApplyDto(postDto, entity);
        await Repository.UpdateAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await Repository.DeleteAsync(id, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
