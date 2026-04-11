namespace Syntra.Behaviors.CircuitBreaker;

/// <summary>
/// Options for <see cref="CircuitBreakerBehavior{TRequest,TResponse}"/>.
/// </summary>
public sealed class CircuitBreakerOptions
{
    /// <summary>Exceptions before the circuit opens.</summary>
    public int ExceptionsAllowedBeforeBreaking { get; set; } = 5;

    /// <summary>How long the circuit stays open before a trial call is permitted.</summary>
    public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);
}
