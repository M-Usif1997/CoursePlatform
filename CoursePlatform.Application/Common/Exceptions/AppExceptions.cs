namespace CoursePlatform.Application.Common.Exceptions;

/// <summary>
/// The request is malformed or fails validation. HTTP 400.
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message)
        : base(message)
    {
    }

    public BadRequestException(string message, string[] errors)
        : base(message) => Errors = errors;

    public string[]? Errors { get; }
}

/// <summary>
/// The request conflicts with the current state of a resource. HTTP 409.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// The caller is authenticated but not allowed to perform the action. HTTP 403.
/// </summary>
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string action)
        : base($"You do not have permission to perform '{action}'.")
    {
    }
}

/// <summary>
/// Credentials or tokens were rejected. HTTP 401.
/// </summary>
public class UnauthenticatedException : Exception
{
    public UnauthenticatedException()
        : base("Username or password is incorrect.")
    {
    }

    public UnauthenticatedException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// A query returned no items at all. HTTP 404.
/// </summary>
public class NoDataFoundException : Exception
{
    public NoDataFoundException()
        : base("No data found.")
    {
    }

    public NoDataFoundException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// A specific entity could not be located. HTTP 404.
/// </summary>
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// A business rule was violated and the message is safe to show to the caller.
/// HTTP 400.
/// </summary>
public class UserFriendlyException : Exception
{
    public UserFriendlyException(string message)
        : base(message)
    {
    }

    public UserFriendlyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
