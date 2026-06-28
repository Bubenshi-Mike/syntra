using Polly;
using Polly.CircuitBreaker;

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
    private static readonly ConcurrentDictionary<string, ResiliencePipeline> Pipelines = new();

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (request is not ICircuitBreakableRequest)
            return await next(cancellationToken).ConfigureAwait(false);

        var key = typeof(TRequest).FullName ?? typeof(TRequest).Name;
        var pipeline = Pipelines.GetOrAdd(key, _ => CreatePipeline(options));

        try
        {
            return await pipeline.ExecuteAsync<TResponse>(
                    async ct => await next(ct).ConfigureAwait(false),
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

    private static ResiliencePipeline CreatePipeline(CircuitBreakerOptions options)
    {
        return new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(static ex => ex is not OperationCanceledException),
                MinimumThroughput = options.ExceptionsAllowedBeforeBreaking,
                FailureRatio = 1.0,
                SamplingDuration = options.SamplingDuration,
                BreakDuration = options.DurationOfBreak,
            })
            .Build();
    }
}
