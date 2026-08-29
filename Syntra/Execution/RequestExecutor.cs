using Ardalis.GuardClauses;
using Syntra.Caching;

namespace Syntra.Execution;

/// <summary>
/// Entry point for command/query dispatch: looks up the cached pipeline for the runtime request type.
/// </summary>
internal static class RequestExecutor
{
    /// <summary>
    /// Dispatches <paramref name="request"/> through behaviors and handler.
    /// </summary>
    public static Task<TResponse> ExecuteAsync<TResponse>(
        IRequest<TResponse> request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);
        Guard.Against.Null(serviceProvider);

        var executor = DelegateCache<TResponse>.GetOrCreateRequestPipeline(request.GetType());
        return executor.HandleAsync(request, serviceProvider, cancellationToken);
    }
}
