namespace Syntra.Exceptions;

/// <summary>
/// Thrown by <c>SyntraMediator</c> when no handler is registered for the given request type.
/// </summary>
/// <remarks>
/// This exception indicates a wiring problem — a handler was not registered in the DI
/// container before a request of that type was dispatched. It is a programming error,
/// not a recoverable runtime error, so it intentionally escapes the Results Pattern.
/// </remarks>
public sealed class HandlerNotFoundException : InvalidOperationException
{
    /// <summary>The request type for which no handler was found.</summary>
    public Type RequestType { get; }

    /// <summary>
    /// Initialises a new instance with a diagnostic message that includes
    /// the fully-qualified request type name.
    /// </summary>
    /// <param name="requestType">The unhandled request type.</param>
    public HandlerNotFoundException(Type requestType)
        : base(
            $"No handler was registered for request type '{requestType.FullName}'. " +
            $"Ensure a class implementing IRequestHandler<{requestType.Name}, TResponse> " +
            $"is registered in the DI container (e.g. via AddSyntra().AddHandlersFromAssembly(...)).")
    {
        RequestType = requestType;
    }
}
