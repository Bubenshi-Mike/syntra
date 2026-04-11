namespace Syntra.Behaviors.Retry;

/// <summary>
/// Options for <see cref="RetryBehavior{TRequest,TResponse}"/>.
/// </summary>
public sealed class RetryOptions
{
    /// <summary>Maximum retry attempts after the first failure.</summary>
    public int MaxRetries { get; set; } = 2;

    /// <summary>Base delay for the first retry (milliseconds).</summary>
    public int BaseDelayMilliseconds { get; set; } = 100;

    /// <summary>Exponential backoff multiplier applied each retry.</summary>
    public double BackoffMultiplier { get; set; } = 2.0;
}
