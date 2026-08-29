namespace Syntra.Abstractions.Handlers;

/// <summary>
/// Handles a streaming query whose items are produced lazily and asynchronously.
/// </summary>
/// <typeparam name="TQuery">The query type. Must implement <see cref="IStreamQuery{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of each item published to the caller.</typeparam>
/// <remarks>
/// Unlike <see cref="IRequestHandler{TRequest, TResponse}"/>, stream handlers return
/// <see cref="IAsyncEnumerable{T}"/> directly, enabling true lazy push-based streaming
/// without buffering the full result set. Consumers iterate with <c>await foreach</c>.
/// </remarks>
public interface IStreamQueryHandler<in TQuery, TResponse>
    where TQuery : IStreamQuery<TResponse>
{
    /// <summary>
    /// Handles the streaming query and returns a lazy async sequence.
    /// </summary>
    /// <param name="query">The query describing what to stream. Never <c>null</c>.</param>
    /// <param name="cancellationToken">Token to observe for cooperative cancellation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <typeparamref name="TResponse"/>.</returns>
    public IAsyncEnumerable<TResponse> HandleAsync(
        TQuery query,
        CancellationToken cancellationToken = default);
}
