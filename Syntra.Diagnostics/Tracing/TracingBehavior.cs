using System.Diagnostics;
using Syntra.Diagnostics.Telemetry;

namespace Syntra.Diagnostics.Tracing;

/// <summary>
/// Creates distributed tracing <see cref="Activity"/> spans around the inner pipeline (opt-in).
/// </summary>
public sealed class TracingBehavior<TRequest, TResponse>(
    ILogger<TracingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly string RequestTypeName = typeof(TRequest).FullName ?? typeof(TRequest).Name;

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.DiagnosticsActivitySource.Instance.StartActivity(
            SyntraActivityNames.MediatorRequest,
            ActivityKind.Internal,
            parentContext: default);

        activity?.SetTag(ActivityTagNames.RequestType, RequestTypeName);

        try
        {
            var response = await next(cancellationToken).ConfigureAwait(false);

            if (response is Result r)
                activity?.SetTag(ActivityTagNames.Success, r.IsSuccess);

            return response;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            logger.LogDebug(ex, "Tracing: exception in pipeline for {RequestType}", RequestTypeName);
            throw;
        }
    }
}
