namespace Syntra.Internal;

/// <summary>
/// Non-generic marker for cached stream handler wrappers.
/// </summary>
internal interface IStreamHandlerWrapper { }

/// <summary>
/// Typed bridge from the non-generic stream dispatch path to the strongly-typed
/// <c>IStreamQueryHandler&lt;TQuery, TResponse&gt;</c>.
/// </summary>
/// <typeparam name="TResponse">The type of each item in the async sequence.</typeparam>
internal interface IStreamHandlerWrapper<TResponse> : IStreamHandlerWrapper
{
    /// <summary>
    /// Executes the stream handler and returns the lazy async sequence.
    /// </summary>
    /// <param name="query">The query, cast internally to the concrete query type.</param>
    /// <param name="serviceProvider">Scoped DI container for resolving the stream handler.</param>
    /// <param name="cancellationToken">Token to observe for cooperative cancellation.</param>
    public IAsyncEnumerable<TResponse> HandleAsync(
        object query,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}
