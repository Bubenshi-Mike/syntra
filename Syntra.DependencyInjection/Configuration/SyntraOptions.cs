using Syntra.Options;

namespace Syntra.DependencyInjection.Configuration;

/// <summary>
/// Root configuration object bound from <c>appsettings.json</c> section <see cref="SectionName"/>
/// and merged with inline <c>Configure(...)</c> calls.
/// </summary>
public sealed class SyntraOptions
{
    /// <summary>Configuration section key (also used by <see cref="Syntra.Options.SyntraMediatorOptions"/>).</summary>
    public const string SectionName = "Syntra";

    /// <summary>Mediator / notification defaults.</summary>
    public MediatorOptionsSection Mediator { get; set; } = new();

    /// <summary>Performance / slow-request logging.</summary>
    public PerformanceOptionsSection Performance { get; set; } = new();

    /// <summary>Retry policy defaults (maps to <see cref="Behaviors.Retry.RetryOptions"/>).</summary>
    public RetryOptionsSection Retry { get; set; } = new();

    /// <summary>Distributed cache defaults for <see cref="Behaviors.Caching.CachingBehavior{TRequest,TResponse}"/>.</summary>
    public CacheOptionsSection Cache { get; set; } = new();

    /// <summary>Timeout pipeline defaults.</summary>
    public TimeoutOptionsSection Timeout { get; set; } = new();

    /// <summary>Circuit breaker defaults.</summary>
    public CircuitBreakerOptionsSection CircuitBreaker { get; set; } = new();

    /// <summary>Authorization behavior defaults.</summary>
    public AuthorizationOptionsSection Authorization { get; set; } = new();

    /// <summary>Idempotency cache defaults.</summary>
    public IdempotencyOptionsSection Idempotency { get; set; } = new();
}

/// <summary>Mediator-related settings (bridged to <see cref="SyntraMediatorOptions"/>).</summary>
public sealed class MediatorOptionsSection
{
    /// <inheritdoc cref="NotificationDispatchStrategy"/>
    public NotificationDispatchStrategy NotificationDispatch { get; set; } = NotificationDispatchStrategy.Sequential;
}

/// <summary>Performance behavior settings.</summary>
public sealed class PerformanceOptionsSection
{
    /// <summary>Milliseconds after which a request is considered slow (warning log).</summary>
    public long QueryThresholdMs { get; set; } = 500;
}

/// <summary>Retry behavior settings.</summary>
public sealed class RetryOptionsSection
{
    /// <summary>Maximum retry attempts (maps to Retry MaxRetries).</summary>
    public int MaxAttempts { get; set; } = 2;

    /// <summary>Base delay between retries in milliseconds.</summary>
    public int BaseDelayMilliseconds { get; set; } = 100;

    /// <summary>Exponential backoff multiplier.</summary>
    public double BackoffMultiplier { get; set; } = 2.0;
}

/// <summary>Cache behavior settings.</summary>
public sealed class CacheOptionsSection
{
    /// <summary>Default TTL when a query does not specify a per-query cache duration.</summary>
    public TimeSpan? DefaultExpiration { get; set; }
}

/// <summary>Timeout behavior settings.</summary>
public sealed class TimeoutOptionsSection
{
    /// <summary>Default pipeline timeout when a request does not implement <see cref="Behaviors.Timeout.ITimeoutRequest"/>.</summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);
}

/// <summary>Circuit breaker settings.</summary>
public sealed class CircuitBreakerOptionsSection
{
    /// <summary>Exceptions before the circuit opens.</summary>
    public int ExceptionsAllowedBeforeBreaking { get; set; } = 5;

    /// <summary>Duration the circuit stays open.</summary>
    public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);
}

/// <summary>Authorization behavior settings.</summary>
public sealed class AuthorizationOptionsSection
{
    /// <inheritdoc cref="Behaviors.Authorization.AuthorizationBehaviorOptions.FailWhenHttpContextMissing"/>
    public bool FailWhenHttpContextMissing { get; set; } = true;
}

/// <summary>Idempotency behavior settings.</summary>
public sealed class IdempotencyOptionsSection
{
    /// <summary>How long idempotent responses are retained.</summary>
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromDays(1);

    /// <summary>Key prefix in the distributed cache.</summary>
    public string CacheKeyPrefix { get; set; } = "syntra:idempotency:";
}
