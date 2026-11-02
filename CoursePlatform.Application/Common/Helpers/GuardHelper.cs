namespace CoursePlatform.Application.Common.Helpers;

/// <summary>
/// Argument guards shared by application services.
/// </summary>
public static class GuardHelper
{
    /// <summary>
    /// Throws when a value is supplied but is not a valid GUID. Null or empty is
    /// treated as "not supplied" and passes.
    /// </summary>
    public static void EnsureValidGuidOrEmpty(string? value, string paramName)
    {
        if (!string.IsNullOrEmpty(value) && !Guid.TryParse(value, out _))
        {
            throw new ArgumentException($"Invalid {paramName}.", paramName);
        }
    }

    /// <summary>
    /// Throws when a required string is null, empty or whitespace.
    /// </summary>
    public static string EnsureNotEmpty(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"'{paramName}' is required.", paramName);
        }

        return value;
    }
}
