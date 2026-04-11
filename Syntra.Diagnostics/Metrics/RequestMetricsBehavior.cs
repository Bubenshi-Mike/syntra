using System.Diagnostics;

namespace Syntra.Diagnostics.Metrics;

/// <summary>
/// Records duration and success/failure metrics for each request (opt-in).
/// </summary>
public sealed class RequestMetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly string RequestType = typeof(TRequest).FullName ?? typeof(TRequest).Name;

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var response = await next(cancellationToken).ConfigureAwait(false);
            sw.Stop();

            var success = response is not Result r || r.IsSuccess;
            SyntraMetrics.RequestDuration.Record(
                sw.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("request.type", RequestType),
                new KeyValuePair<string, object?>("success", success));

            SyntraMetrics.RequestTotal.Add(1, new KeyValuePair<string, object?>("request.type", RequestType));

            if (!success && response is Result failed)
            {
                SyntraMetrics.RequestFailures.Add(
                    1,
                    new KeyValuePair<string, object?>("request.type", RequestType),
                    new KeyValuePair<string, object?>("error.type", failed.Error?.Type.ToString() ?? "unknown"));
            }

            return response;
        }
        catch
        {
            sw.Stop();
            SyntraMetrics.RequestDuration.Record(
                sw.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("request.type", RequestType),
                new KeyValuePair<string, object?>("success", false));
            SyntraMetrics.RequestTotal.Add(1, new KeyValuePair<string, object?>("request.type", RequestType));
            SyntraMetrics.RequestFailures.Add(
                1,
                new KeyValuePair<string, object?>("request.type", RequestType),
                new KeyValuePair<string, object?>("error.type", "exception"));
            throw;
        }
    }
}
