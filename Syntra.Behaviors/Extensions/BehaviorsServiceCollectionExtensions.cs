using Microsoft.Extensions.DependencyInjection.Extensions;
using Syntra.Abstractions.Pipelines;
using Syntra.Behaviors.Audit;
using Syntra.Behaviors.Authorization;
using Syntra.Behaviors.Caching;
using Syntra.Behaviors.CircuitBreaker;
using Syntra.Behaviors.ExceptionHandling;
using Syntra.Behaviors.Idempotency;
using Syntra.Behaviors.Logging;
using Syntra.Behaviors.Performance;
using Syntra.Behaviors.Retry;
using Syntra.Behaviors.Timeout;
using Syntra.Behaviors.Transactions;
using Syntra.Behaviors.Validation;

namespace Syntra.Behaviors.Extensions;

/// <summary>
/// Opt-in registration for individual pipeline behaviors (each folder is intended for future split packages).
/// </summary>
public static class BehaviorsServiceCollectionExtensions
{
    /// <summary>Registers <see cref="LoggingBehavior{TRequest,TResponse}"/>.</summary>
    public static IServiceCollection AddLoggingBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return services;
    }

    /// <summary>Registers <see cref="ExceptionBehavior{TRequest,TResponse}"/>.</summary>
    public static IServiceCollection AddExceptionBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
        return services;
    }

    /// <summary>Registers <see cref="TimeoutBehavior{TRequest,TResponse}"/> and <see cref="TimeoutOptions"/>.</summary>
    public static IServiceCollection AddTimeoutBehavior(
        this IServiceCollection services,
        Action<TimeoutOptions>? configure = null)
    {
        services.TryAddSingleton<TimeoutOptions>(_ =>
        {
            var o = new TimeoutOptions();
            configure?.Invoke(o);
            return o;
        });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TimeoutBehavior<,>));
        return services;
    }

    /// <summary>Registers <see cref="CircuitBreakerBehavior{TRequest,TResponse}"/>.</summary>
    /// <param name="behaviorLifetime">
    /// Use <see cref="ServiceLifetime.Singleton"/> for a single policy instance per closed generic (recommended for Polly policy caches).
    /// </param>
    public static IServiceCollection AddCircuitBreakerBehavior(
        this IServiceCollection services,
        Action<CircuitBreakerOptions>? configure = null,
        ServiceLifetime behaviorLifetime = ServiceLifetime.Transient)
    {
        services.TryAddSingleton<CircuitBreakerOptions>(_ =>
        {
            var o = new CircuitBreakerOptions();
            configure?.Invoke(o);
            return o;
        });
        services.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(CircuitBreakerBehavior<,>), behaviorLifetime));
        return services;
    }

    /// <summary>Registers <see cref="RetryBehavior{TRequest,TResponse}"/>.</summary>
    /// <param name="behaviorLifetime">
    /// Use <see cref="ServiceLifetime.Singleton"/> for a single policy instance per closed generic (recommended for Polly policy caches).
    /// </param>
    public static IServiceCollection AddRetryBehavior(
        this IServiceCollection services,
        Action<RetryOptions>? configure = null,
        ServiceLifetime behaviorLifetime = ServiceLifetime.Transient)
    {
        services.TryAddSingleton<RetryOptions>(_ =>
        {
            var o = new RetryOptions();
            configure?.Invoke(o);
            return o;
        });
        services.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>), behaviorLifetime));
        return services;
    }

    /// <summary>Registers <see cref="PerformanceBehavior{TRequest,TResponse}"/>.</summary>
    public static IServiceCollection AddPerformanceBehavior(
        this IServiceCollection services,
        Action<PerformanceBehaviorOptions>? configure = null)
    {
        services.TryAddSingleton<PerformanceBehaviorOptions>(_ =>
        {
            var o = new PerformanceBehaviorOptions();
            configure?.Invoke(o);
            return o;
        });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        return services;
    }

    /// <summary>
    /// Registers <see cref="AuthorizationBehavior{TRequest,TResponse}"/> (requires ASP.NET Core auth services + <see cref="Microsoft.AspNetCore.Http.IHttpContextAccessor"/>).
    /// </summary>
    public static IServiceCollection AddAuthorizationBehavior(
        this IServiceCollection services,
        Action<AuthorizationBehaviorOptions>? configure = null)
    {
        services.TryAddSingleton<AuthorizationBehaviorOptions>(_ =>
        {
            var o = new AuthorizationBehaviorOptions();
            configure?.Invoke(o);
            return o;
        });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        return services;
    }

    /// <summary>Registers <see cref="ValidationBehavior{TRequest,TResponse}"/>.</summary>
    public static IServiceCollection AddValidationBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }

    /// <summary>Registers <see cref="CachingBehavior{TRequest,TResponse}"/> and <see cref="ICacheKeyGenerator"/>.</summary>
    public static IServiceCollection AddCachingBehavior(
        this IServiceCollection services,
        Action<CacheOptions>? configure = null)
    {
        services.TryAddSingleton<CacheOptions>(_ =>
        {
            var o = new CacheOptions();
            configure?.Invoke(o);
            return o;
        });
        services.TryAddSingleton<ICacheKeyGenerator>(sp =>
            new DefaultCacheKeyGenerator(sp.GetRequiredService<CacheOptions>()));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        return services;
    }

    /// <summary>Registers <see cref="IdempotencyBehavior{TRequest,TResponse}"/>.</summary>
    public static IServiceCollection AddIdempotencyBehavior(
        this IServiceCollection services,
        Action<IdempotencyOptions>? configure = null)
    {
        services.TryAddSingleton<IdempotencyOptions>(_ =>
        {
            var o = new IdempotencyOptions();
            configure?.Invoke(o);
            return o;
        });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
        return services;
    }

    /// <summary>Registers <see cref="TransactionBehavior{TRequest,TResponse}"/> (requires <see cref="ITransactionManager"/>).</summary>
    public static IServiceCollection AddTransactionBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        return services;
    }

    /// <summary>Registers <see cref="AuditBehavior{TRequest,TResponse}"/> (requires <see cref="IAuditWriter"/>).</summary>
    public static IServiceCollection AddAuditBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));
        return services;
    }

    /// <summary>
    /// Registers a recommended outer-to-inner stack (excludes <see cref="AuditBehavior{TRequest,TResponse}"/> —
    /// call <see cref="AddAuditBehavior"/> after registering <see cref="IAuditWriter"/>).
    /// </summary>
    public static IServiceCollection AddStandardSyntraBehaviors(
        this IServiceCollection services,
        Action<PerformanceBehaviorOptions>? configurePerformance = null,
        Action<TimeoutOptions>? configureTimeout = null,
        Action<CircuitBreakerOptions>? configureCircuitBreaker = null,
        Action<RetryOptions>? configureRetry = null,
        Action<AuthorizationBehaviorOptions>? configureAuthorization = null,
        Action<CacheOptions>? configureCache = null,
        Action<IdempotencyOptions>? configureIdempotency = null)
    {
        services
            .AddLoggingBehavior()
            .AddExceptionBehavior()
            .AddTimeoutBehavior(configureTimeout)
            .AddCircuitBreakerBehavior(configureCircuitBreaker)
            .AddRetryBehavior(configureRetry)
            .AddPerformanceBehavior(configurePerformance)
            .AddAuthorizationBehavior(configureAuthorization)
            .AddValidationBehavior()
            .AddCachingBehavior(configureCache)
            .AddIdempotencyBehavior(configureIdempotency)
            .AddTransactionBehavior();

        return services;
    }
}
