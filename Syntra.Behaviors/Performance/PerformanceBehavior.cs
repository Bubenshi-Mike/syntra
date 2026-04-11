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
            logger.LogWarning(
                "Long-running request detected: {RequestName} took {ElapsedMs}ms (threshold: {ThresholdMs}ms). Request: {@Request}",
                RequestName,
                sw.ElapsedMilliseconds,
                options.ThresholdMilliseconds,
                request);
        }

        return response;
    }
}
