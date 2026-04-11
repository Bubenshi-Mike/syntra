namespace Syntra.Exceptions;

/// <summary>
/// Thrown when the pipeline encounters an unrecoverable structural error,
/// such as a behavior implementation that returns <c>null</c> or a misconfigured
/// composition that breaks the delegate chain.
/// </summary>
/// <remarks>
/// This exception represents a programming error in behavior configuration,
/// not a recoverable domain failure. It intentionally escapes the Results Pattern.
/// </remarks>
public sealed class PipelineException : InvalidOperationException
{
    /// <summary>The request type that was being processed when the error occurred.</summary>
    public Type? RequestType { get; }

    /// <summary>
    /// Initialises a new instance with a diagnostic message.
    /// </summary>
    public PipelineException(string message) : base(message) { }

    /// <summary>
    /// Initialises a new instance with a diagnostic message and the request type.
    /// </summary>
    public PipelineException(string message, Type requestType) : base(message)
    {
        RequestType = requestType;
    }

    /// <summary>
    /// Initialises a new instance with a diagnostic message and an inner exception.
    /// </summary>
    public PipelineException(string message, Exception innerException)
        : base(message, innerException) { }
}
