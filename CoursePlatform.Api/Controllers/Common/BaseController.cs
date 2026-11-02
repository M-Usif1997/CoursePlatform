using CoursePlatform.Application.Services.Base;
using CoursePlatform.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.Api.Controllers.Common;

/// <summary>
/// Generic CRUD endpoints for a feature. A feature controller derives from this
/// type and passes its entity and DTOs:
/// <code>
/// public sealed class OrdersController : BaseController&lt;Order, OrderDto, PostOrderDto&gt;
/// {
///     public OrdersController(IOrderService service) : base(service) { }
/// }
/// </code>
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public abstract class BaseController<TEntity, TGetDto, TPostDto> : ControllerBase
    where TEntity : BaseEntity
    where TGetDto : class
    where TPostDto : class
{
    protected BaseController(IGenericBaseService<TEntity, TGetDto, TPostDto> service)
    {
        Service = service;
    }

    protected IGenericBaseService<TEntity, TGetDto, TPostDto> Service { get; }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await Service.GetByIdAsync(id, cancellationToken));

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await Service.GetAllAsync(cancellationToken));

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Search(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = PagedRequest.DefaultPageSize,
        CancellationToken cancellationToken = default)
        => Ok(await Service.SearchAsync(
            new PagedRequest { PageNumber = pageNumber, PageSize = pageSize },
            cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> Create(
        [FromBody] TPostDto postDto,
        CancellationToken cancellationToken)
    {
        var created = await Service.CreateAsync(postDto, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> CreateRange(
        [FromBody] IEnumerable<TPostDto> postDtos,
        CancellationToken cancellationToken)
    {
        var created = await Service.CreateRangeAsync(postDtos, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> Update(
        Guid id,
        [FromBody] TPostDto postDto,
        CancellationToken cancellationToken)
    {
        await Service.UpdateAsync(id, postDto, cancellationToken);

        return Ok(new { id });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Service.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
