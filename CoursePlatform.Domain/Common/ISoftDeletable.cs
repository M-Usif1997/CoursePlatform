namespace CoursePlatform.Domain.Common;

/// <summary>
/// Marks an entity that is never physically removed. A delete request flips
/// <see cref="IsDeleted"/> and the entity is filtered out of normal queries.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
