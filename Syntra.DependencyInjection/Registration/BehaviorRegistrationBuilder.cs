using Microsoft.Extensions.DependencyInjection;
using Syntra.Behaviors.Extensions;
using Syntra.Diagnostics.Extensions;

namespace Syntra.DependencyInjection.Registration;

/// <summary>
/// Fluent registration of built-in pipeline behaviors (order = outermost first).
/// </summary>
public sealed class BehaviorRegistrationBuilder
{
    internal List<Action<IServiceCollection>> Steps { get; } = new();

    /// <summary>
    /// Registers the default outer-to-inner stack matching the fluent API sample (exception → logging → … → audit).
    /// </summary>
    public BehaviorRegistrationBuilder AddStandardPipeline(bool includeAudit = true)
    {
        AddException();
        AddLogging();
        AddAuthorization();
        AddValidation();
        AddIdempotency();
        AddCaching();
        AddPerformance();
        AddRetry(ServiceLifetime.Singleton);
        AddCircuitBreaker(ServiceLifetime.Singleton);
        AddTimeout();
        AddTransaction();
        if (includeAudit)
            AddAudit();
        return this;
    }

    /// <inheritdoc cref="BehaviorsServiceCollectionExtensions.AddExceptionBehavior"/>
    public BehaviorRegistrationBuilder AddException()
    {
        Steps.Add(s => s.AddExceptionBehavior());
        return this;
    }

    /// <inheritdoc cref="BehaviorsServiceCollectionExtensions.AddLoggingBehavior"/>
    public BehaviorRegistrationBuilder AddLogging()
    {
        Steps.Add(s => s.AddLoggingBehavior());
        return this;
    }

    /// <inheritdoc cref="BehaviorsServiceCollectionExtensions.AddAuthorizationBehavior"/>
    public BehaviorRegistrationBuilder AddAuthorization()
    {
        Steps.Add(s => s.AddAuthorizationBehavior());
        return this;
    }

    /// <inheritdoc cref="BehaviorsServiceCollectionExtensions.AddValidationBehavior"/>
    public BehaviorRegistrationBuilder AddValidation()
    {
        Steps.Add(s => s.AddValidationBehavior());
        return this;
    }

    /// <inheritdoc cref="BehaviorsServiceCollectionExtensions.AddPerformanceBehavior"/>
    public BehaviorRegistrationBuilder AddPerformance()
    {
        Steps.Add(s => s.AddPerformanceBehavior());
        return this;
    }

    /// <inheritdoc cref="BehaviorsServiceCollectionExtensions.AddCachingBehavior"/>
    public BehaviorRegistrationBuilder AddCaching()
    {
        Steps.Add(s => s.AddCachingBehavior());
        return this;
    }

    /// <inheritdoc cref="BehaviorsServiceCollectionExtensions.AddTransactionBehavior"/>
    public BehaviorRegistrationBuilder AddTransaction()
    {
        Steps.Add(s => s.AddTransactionBehavior());
        return this;
    }

    /// <inheritdoc cref="BehaviorsServiceCollectionExtensions.AddIdempotencyBehavior"/>
    public BehaviorRegistrationBuilder AddIdempotency()
    {
        Steps.Add(s => s.AddIdempotencyBehavior());
        return this;
    }

    /// <inheritdoc cref="BehaviorsServiceCollectionExtensions.AddRetryBehavior"/>
    public BehaviorRegistrationBuilder AddRetry(ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        Steps.Add(s => s.AddRetryBehavior(behaviorLifetime: lifetime));
        return this;
    }

    /// <inheritdoc cref="BehaviorsServiceCollectionExtensions.AddCircuitBreakerBehavior"/>
    public BehaviorRegistrationBuilder AddCircuitBreaker(ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        Steps.Add(s => s.AddCircuitBreakerBehavior(behaviorLifetime: lifetime));
        return this;
    }

    /// <inheritdoc cref="BehaviorsServiceCollectionExtensions.AddTimeoutBehavior"/>
    public BehaviorRegistrationBuilder AddTimeout()
    {
        Steps.Add(s => s.AddTimeoutBehavior());
        return this;
    }

    /// <inheritdoc cref="BehaviorsServiceCollectionExtensions.AddAuditBehavior"/>
    public BehaviorRegistrationBuilder AddAudit()
    {
        Steps.Add(s => s.AddAuditBehavior());
        return this;
    }

    /// <summary>
    /// Registers Syntra's opt-in tracing (<see cref="Syntra.Diagnostics.Tracing.TracingBehavior{TRequest,TResponse}"/>)
    /// and metrics (<see cref="Syntra.Diagnostics.Metrics.RequestMetricsBehavior{TRequest,TResponse}"/>) behaviors.
    /// </summary>
    /// <inheritdoc cref="SyntraDiagnosticsExtensions.AddSyntraDiagnostics"/>
    public BehaviorRegistrationBuilder AddDiagnostics()
    {
        Steps.Add(s => s.AddSyntraDiagnostics());
        return this;
    }
}
