namespace CoursePlatform.Domain.Common;

/// <summary>
/// Marks an entity whose creation and modification metadata is maintained
/// automatically when it is persisted.
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; set; }

    string? CreatedBy { get; set; }

    DateTime? LastModifiedAtUtc { get; set; }

    string? LastModifiedBy { get; set; }
}
