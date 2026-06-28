namespace Syntra.Behaviors.CircuitBreaker;

/// <summary>
/// Options for <see cref="CircuitBreakerBehavior{TRequest,TResponse}"/>.
/// </summary>
public sealed class CircuitBreakerOptions
{
    /// <summary>Minimum number of failures within <see cref="SamplingDuration"/> required to open the circuit.</summary>
    public int ExceptionsAllowedBeforeBreaking { get; set; } = 5;

    /// <summary>Time window in which failures are counted toward <see cref="ExceptionsAllowedBeforeBreaking"/>.</summary>
    public TimeSpan SamplingDuration { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>How long the circuit stays open before a trial call is permitted.</summary>
    public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);
}
