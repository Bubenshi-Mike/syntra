using Ardalis.GuardClauses;
using Syntra.Caching;

namespace Syntra.Execution;

/// <summary>
/// Entry point for streaming queries: looks up the cached executor for the runtime query type.
/// </summary>
internal static class StreamExecutor
{
    /// <summary>
    /// Dispatches <paramref name="query"/> to its stream handler.
    /// </summary>
    public static IAsyncEnumerable<TResponse> ExecuteAsync<TResponse>(
        IStreamQuery<TResponse> query,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(query);
        Guard.Against.Null(serviceProvider);

        var executor = StreamDelegateCache<TResponse>.GetOrCreate(query.GetType());
        return executor.HandleAsync(query, serviceProvider, cancellationToken);
    }
}
