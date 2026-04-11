namespace Syntra.DependencyInjection.Options;

/// <summary>
/// Optional override for the logical pipeline order when using convention-based registration.
/// The default order matches <see cref="Registration.BehaviorRegistrationBuilder.AddStandardPipeline"/>.
/// </summary>
/// <remarks>
/// First in the list is the outermost <see cref="Abstractions.Pipelines.IPipelineBehavior{TRequest,TResponse}"/> (runs first).
/// </remarks>
public sealed class BehaviorOrderOptions
{
    /// <summary>
    /// When non-empty, replaces the default behavior registration sequence.
    /// </summary>
    public IReadOnlyList<BehaviorSlot>? CustomOrder { get; set; }
}

/// <summary>Identifies a built-in behavior slot for ordering.</summary>
public enum BehaviorSlot
{
    Logging,
    Exception,
    Timeout,
    CircuitBreaker,
    Retry,
    Performance,
    Authorization,
    Validation,
    Caching,
    Idempotency,
    Transaction,
    Audit,
}
