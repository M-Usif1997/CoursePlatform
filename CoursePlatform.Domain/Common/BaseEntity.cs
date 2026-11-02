namespace CoursePlatform.Domain.Common;

/// <summary>
/// Base type for every aggregate in the domain. Provides identity, audit
/// metadata and soft delete support.
/// </summary>
public abstract class BaseEntity : IAuditableEntity, ISoftDeletable
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? LastModifiedAtUtc { get; set; }

    public string? LastModifiedBy { get; set; }

    public bool IsDeleted { get; set; }
}
