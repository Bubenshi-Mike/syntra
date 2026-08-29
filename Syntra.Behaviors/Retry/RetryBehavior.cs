using Polly;
using Polly.Retry;

namespace Syntra.Behaviors.Retry;

/// <summary>
/// Retries the inner pipeline for requests implementing <see cref="IRetryableRequest"/> using Polly retry.
/// </summary>
/// <remarks>
/// One <see cref="ResiliencePipeline"/> instance is cached per request type full name.
/// </remarks>
public sealed class RetryBehavior<TRequest, TResponse>(
    RetryOptions options,
    ILogger<RetryBehavior<TRequest, TResponse>> logger)
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
        if (request is not IRetryableRequest)
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }

        var key = typeof(TRequest).FullName ?? typeof(TRequest).Name;
        var pipeline = Pipelines.GetOrAdd(key, _ => CreatePipeline(options, logger));
        return await pipeline.ExecuteAsync<TResponse>(
                async ct => await next(ct).ConfigureAwait(false),
                cancellationToken)
            .ConfigureAwait(false);
    }

    private static ResiliencePipeline CreatePipeline(RetryOptions options, ILogger logger)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(static ex => ex is not OperationCanceledException),
                MaxRetryAttempts = options.MaxRetries,
                DelayGenerator = args =>
                {
                    var factor = Math.Pow(options.BackoffMultiplier, args.AttemptNumber);
                    return new ValueTask<TimeSpan?>(TimeSpan.FromMilliseconds(options.BaseDelayMilliseconds * factor));
                },
                OnRetry = args =>
                {
                    // Do not pass the exception directly — many providers print a full stack trace per retry,
                    // which drowns logs when retries are expected (transient faults).
                    logger.LogWarning(
                        "Retry attempt {Attempt} after {Delay}ms due to {ExceptionType}: {Message}",
                        args.AttemptNumber + 1,
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome.Exception?.GetType().Name,
                        args.Outcome.Exception?.Message);
                    return default;
                }
            })
            .Build();
    }
}
