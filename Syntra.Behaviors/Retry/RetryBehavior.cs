using Polly;

namespace Syntra.Behaviors.Retry;

/// <summary>
/// Retries the inner pipeline for requests implementing <see cref="IRetryableRequest"/> using Polly <c>WaitAndRetryAsync</c>.
/// </summary>
/// <remarks>
/// One <see cref="AsyncPolicy"/> instance is cached per request type full name.
/// </remarks>
public sealed class RetryBehavior<TRequest, TResponse>(
    RetryOptions options,
    ILogger<RetryBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly ConcurrentDictionary<string, AsyncPolicy> Policies = new();

    /// <inheritdoc />
    public Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (request is not IRetryableRequest)
            return next(cancellationToken);

        var key = typeof(TRequest).FullName ?? typeof(TRequest).Name;
        var policy = Policies.GetOrAdd(key, _ => CreatePolicy(options, logger));
        return policy.ExecuteAsync<TResponse>(ct => next(ct), cancellationToken);
    }

    private static AsyncPolicy CreatePolicy(RetryOptions options, ILogger logger)
    {
        return (AsyncPolicy)Policy
            .Handle<Exception>(static ex => ex is not OperationCanceledException)
            .WaitAndRetryAsync(
                options.MaxRetries,
                retryAttempt =>
                {
                    var factor = Math.Pow(options.BackoffMultiplier, retryAttempt - 1);
                    return TimeSpan.FromMilliseconds(options.BaseDelayMilliseconds * factor);
                },
                onRetry: (ex, delay, attempt, _) =>
                {
                    // Do not pass `ex` to LogWarning — many providers print a full stack trace per retry,
                    // which drowns logs when retries are expected (transient faults).
                    logger.LogWarning(
                        "Retry attempt {Attempt} after {Delay}ms due to {ExceptionType}: {Message}",
                        attempt,
                        delay.TotalMilliseconds,
                        ex.GetType().Name,
                        ex.Message);
                });
    }
}
