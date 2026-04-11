namespace Syntra.Abstractions.Results;

/// <summary>
/// Categorises the kind of failure a <see cref="Error"/> describes.
/// Used by pipeline behaviors, mappers, and HTTP adapters to make
/// routing decisions without inspecting error codes as strings.
/// </summary>
public enum ErrorType
{
    /// <summary>Generic, uncategorised failure.</summary>
    Failure = 0,

    /// <summary>
    /// One or more input values did not satisfy validation rules.
    /// Maps to HTTP 422 Unprocessable Entity / 400 Bad Request.
    /// </summary>
    Validation = 1,

    /// <summary>
    /// The requested resource could not be found.
    /// Maps to HTTP 404 Not Found.
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// The operation conflicts with an existing resource state (e.g. duplicate key).
    /// Maps to HTTP 409 Conflict.
    /// </summary>
    Conflict = 3,

    /// <summary>
    /// The caller is not authenticated.
    /// Maps to HTTP 401 Unauthorized.
    /// </summary>
    Unauthorized = 4,

    /// <summary>
    /// The caller is authenticated but lacks permission.
    /// Maps to HTTP 403 Forbidden.
    /// </summary>
    Forbidden = 5,

    /// <summary>
    /// An unexpected, unrecoverable condition occurred (e.g. unhandled exception).
    /// Maps to HTTP 500 Internal Server Error.
    /// </summary>
    Unexpected = 6,
}
