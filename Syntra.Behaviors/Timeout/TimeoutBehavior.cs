namespace Syntra.Behaviors.Timeout;

/// <summary>
/// Applies a cooperative deadline for requests implementing <see cref="ITimeoutRequest"/>.
/// When <see cref="ITimeoutRequest.Timeout"/> is <c>null</c>, <see cref="TimeoutOptions.DefaultTimeout"/> is used.
/// </summary>
public sealed class TimeoutBehavior<TRequest, TResponse>(
    TimeoutOptions options,
    ILogger<TimeoutBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (request is not ITimeoutRequest timeoutRequest)
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }

        var timeout = timeoutRequest.Timeout ?? options.DefaultTimeout;

        if (timeout <= TimeSpan.Zero)
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linked.CancelAfter(timeout);

        try
        {
            return await next(linked.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(
                "Request {RequestType} exceeded timeout of {Timeout}",
                typeof(TRequest).Name,
                timeout);
            throw;
        }
    }
}
