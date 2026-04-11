namespace Syntra.Abstractions.Results;

/// <summary>
/// An immutable, value-typed description of why an operation failed.
/// Carries a machine-readable <see cref="Code"/>, a human-readable
/// <see cref="Message"/>, and a <see cref="Type"/> that lets infrastructure
/// layers (HTTP adapters, loggers, retry policies) make routing decisions
/// without pattern-matching on strings.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Error"/> is a <c>sealed record</c>, so structural equality,
/// non-destructive mutation (<c>with</c> expressions), and deconstruction
/// are available out of the box.
/// </para>
/// <para>
/// Factory methods (<see cref="Validation"/>, <see cref="NotFound"/>, etc.)
/// are the preferred construction path; they encode the <see cref="ErrorType"/>
/// automatically.
/// </para>
/// </remarks>
/// <param name="Code">
/// A dot-separated, PascalCase identifier for the error, e.g.
/// <c>"Order.NotFound"</c> or <c>"User.Email.Invalid"</c>.
/// </param>
/// <param name="Message">Human-readable description of the failure.</param>
/// <param name="Type">Broad category of the error; defaults to <see cref="ErrorType.Failure"/>.</param>
public sealed record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
{
    // -------------------------------------------------------------------------
    // Sentinel
    // -------------------------------------------------------------------------

    /// <summary>
    /// The "no error" sentinel used internally to represent a successful result.
    /// Consumer code should never need to reference this directly.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    // -------------------------------------------------------------------------
    // Factory — generic failure
    // -------------------------------------------------------------------------

    /// <summary>Creates a generic <see cref="ErrorType.Failure"/> error.</summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="message">Human-readable description.</param>
    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    // -------------------------------------------------------------------------
    // Factory — validation
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a <see cref="ErrorType.Validation"/> error, optionally scoped to a field.
    /// </summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="message">Human-readable description.</param>
    /// <param name="fieldName">
    /// Optional field or property name that failed validation.
    /// When provided, the message is prefixed with <c>"&lt;fieldName&gt;: "</c>.
    /// </param>
    public static Error Validation(string code, string message, string? fieldName = null) =>
        new(code, fieldName is null ? message : $"{fieldName}: {message}", ErrorType.Validation);

    // -------------------------------------------------------------------------
    // Factory — not found
    // -------------------------------------------------------------------------

    /// <summary>Creates a <see cref="ErrorType.NotFound"/> error.</summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="message">Human-readable description.</param>
    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    // -------------------------------------------------------------------------
    // Factory — conflict
    // -------------------------------------------------------------------------

    /// <summary>Creates a <see cref="ErrorType.Conflict"/> error.</summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="message">Human-readable description.</param>
    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    // -------------------------------------------------------------------------
    // Factory — authorization
    // -------------------------------------------------------------------------

    /// <summary>Creates a <see cref="ErrorType.Unauthorized"/> error.</summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="message">Human-readable description.</param>
    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    /// <summary>Creates a <see cref="ErrorType.Forbidden"/> error.</summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="message">Human-readable description.</param>
    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);

    // -------------------------------------------------------------------------
    // Factory — unexpected / exception
    // -------------------------------------------------------------------------

    /// <summary>Creates an <see cref="ErrorType.Unexpected"/> error.</summary>
    /// <param name="code">Machine-readable error code.</param>
    /// <param name="message">Human-readable description.</param>
    public static Error Unexpected(string code, string message) =>
        new(code, message, ErrorType.Unexpected);

    /// <summary>
    /// Creates an <see cref="ErrorType.Unexpected"/> error from an exception.
    /// The error code is the exception's type name; the message is <see cref="Exception.Message"/>.
    /// </summary>
    /// <param name="exception">The exception to convert.</param>
    /// <param name="includeStackTrace">
    /// When <c>true</c>, appends the stack trace to <see cref="Message"/>.
    /// Avoid in production — stack traces in error messages can leak internals.
    /// </param>
    public static Error FromException(Exception exception, bool includeStackTrace = false)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var message = includeStackTrace
            ? $"{exception.Message}{Environment.NewLine}{exception.StackTrace}"
            : exception.Message;

        return new(exception.GetType().Name, message, ErrorType.Unexpected);
    }

    // -------------------------------------------------------------------------
    // Formatting
    // -------------------------------------------------------------------------

    /// <inheritdoc />
    public override string ToString() => $"[{Type}] {Code}: {Message}";
}
