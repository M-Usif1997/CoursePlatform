using CoursePlatform.Application.Common.Interfaces;
using CoursePlatform.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CoursePlatform.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Stamps audit columns on every save and converts physical deletes of
/// <see cref="ISoftDeletable"/> entities into an update.
/// </summary>
public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuditableEntityInterceptor(
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
    {
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            ApplyAuditRules(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            ApplyAuditRules(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditRules(DbContext context)
    {
        var utcNow = _dateTimeProvider.UtcNow;
        var actor = _currentUserService.Email ?? _currentUserService.UserId?.ToString();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added when entry.Entity is IAuditableEntity added:
                    added.CreatedAtUtc = utcNow;
                    added.CreatedBy = actor;
                    break;

                case EntityState.Modified when entry.Entity is IAuditableEntity modified:
                    modified.LastModifiedAtUtc = utcNow;
                    modified.LastModifiedBy = actor;
                    break;

                case EntityState.Deleted when entry.Entity is ISoftDeletable deleted:
                    entry.State = EntityState.Modified;
                    deleted.IsDeleted = true;

                    if (entry.Entity is IAuditableEntity softDeleted)
                    {
                        softDeleted.LastModifiedAtUtc = utcNow;
                        softDeleted.LastModifiedBy = actor;
                    }

                    break;
            }
        }
    }
}
