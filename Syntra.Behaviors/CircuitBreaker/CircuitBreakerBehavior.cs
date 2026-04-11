using Polly;
using Polly.CircuitBreaker;
using Syntra.Behaviors.Shared;

namespace Syntra.Behaviors.CircuitBreaker;

/// <summary>
/// Applies a Polly circuit breaker per request type for <see cref="ICircuitBreakableRequest"/> requests.
/// </summary>
public sealed class CircuitBreakerBehavior<TRequest, TResponse>(
    CircuitBreakerOptions options,
    ILogger<CircuitBreakerBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly ConcurrentDictionary<string, AsyncPolicy> Policies = new();

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (request is not ICircuitBreakableRequest)
            return await next(cancellationToken).ConfigureAwait(false);

        var key = typeof(TRequest).FullName ?? typeof(TRequest).Name;
        var policy = Policies.GetOrAdd(key, _ => CreatePolicy(options));

        try
        {
            return await policy.ExecuteAsync<TResponse>(
                    ct => next(ct),
                    cancellationToken)
                .ConfigureAwait(false);
        }
        catch (BrokenCircuitException ex)
        {
            logger.LogWarning(ex, "Circuit breaker open for {RequestType}", typeof(TRequest).Name);
            var error = Error.Unexpected(
                "CircuitBreaker.Open",
                "The operation is temporarily unavailable. Please try again later.");
            return ResultFailureMapper.CreateFailureResult<TResponse>(error);
        }
    }

    private static AsyncPolicy CreatePolicy(CircuitBreakerOptions options)
    {
        return (AsyncPolicy)Policy
            .Handle<Exception>(static ex => ex is not OperationCanceledException)
            .CircuitBreakerAsync(
                options.ExceptionsAllowedBeforeBreaking,
                options.DurationOfBreak);
    }
}
