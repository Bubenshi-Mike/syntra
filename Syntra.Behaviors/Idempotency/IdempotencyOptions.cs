namespace Syntra.Behaviors.Idempotency;

/// <summary>
/// Options for <see cref="IdempotencyBehavior{TRequest,TResponse}"/>.
/// </summary>
public sealed class IdempotencyOptions
{
    /// <summary>Prefix for distributed-cache keys.</summary>
    public string CacheKeyPrefix { get; set; } = "syntra:idempotency:";

    /// <summary>How long completed responses are retained for duplicate detection.</summary>
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromDays(1);
}
