using System.Diagnostics;

namespace Syntra.Behaviors.Performance;

/// <summary>
/// Emits a warning log when request handling exceeds <see cref="PerformanceBehaviorOptions.ThresholdMilliseconds"/>.
/// </summary>
public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
    PerformanceBehaviorOptions options)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly string RequestName = typeof(TRequest).Name;

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var response = await next(cancellationToken).ConfigureAwait(false);
        sw.Stop();

        if (sw.ElapsedMilliseconds >= options.ThresholdMilliseconds)
        {
            // Deliberately does not log the request object itself (e.g. via {@Request}
            // destructuring) - Syntra has no way to know which properties on an arbitrary,
            // consumer-defined request type are sensitive (passwords, tokens, PII), so logging
            // the full object by default would leak whatever the request happens to contain.
            logger.LogWarning(
                "Long-running request detected: {RequestName} took {ElapsedMs}ms (threshold: {ThresholdMs}ms)",
                RequestName,
                sw.ElapsedMilliseconds,
                options.ThresholdMilliseconds);
        }

        return response;
    }
}
