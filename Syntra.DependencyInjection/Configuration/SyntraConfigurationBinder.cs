using Microsoft.Extensions.Options;
using Syntra.Behaviors.Authorization;
using Syntra.Behaviors.Caching;
using Syntra.Behaviors.CircuitBreaker;
using Syntra.Behaviors.Idempotency;
using Syntra.Behaviors.Performance;
using Syntra.Behaviors.Retry;
using Syntra.Behaviors.Timeout;
using Syntra.Options;

namespace Syntra.DependencyInjection.Configuration;

/// <summary>
/// Wires <see cref="SyntraOptions"/> to concrete Microsoft.Extensions.Options targets used by behaviors and the mediator.
/// </summary>
public static class SyntraConfigurationBinder
{
    /// <summary>
    /// Registers <see cref="IConfigureOptions{TOptions}"/> bridges from <see cref="SyntraOptions"/>.</summary>
    public static void RegisterOptionBridges(IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<SyntraMediatorOptions>, SyntraMediatorOptionsFromRoot>();
        services.AddSingleton<IConfigureOptions<PerformanceBehaviorOptions>, PerformanceBehaviorOptionsFromRoot>();
        services.AddSingleton<IConfigureOptions<RetryOptions>, RetryOptionsFromRoot>();
        services.AddSingleton<IConfigureOptions<CacheOptions>, CacheOptionsFromRoot>();
        services.AddSingleton<IConfigureOptions<TimeoutOptions>, TimeoutOptionsFromRoot>();
        services.AddSingleton<IConfigureOptions<CircuitBreakerOptions>, CircuitBreakerOptionsFromRoot>();
        services.AddSingleton<IConfigureOptions<AuthorizationBehaviorOptions>, AuthorizationBehaviorOptionsFromRoot>();
        services.AddSingleton<IConfigureOptions<IdempotencyOptions>, IdempotencyOptionsFromRoot>();
    }

    private sealed class SyntraMediatorOptionsFromRoot(IOptions<SyntraOptions> root) : IConfigureOptions<SyntraMediatorOptions>
    {
        public void Configure(SyntraMediatorOptions options)
        {
            options.NotificationDispatch = root.Value.Mediator.NotificationDispatch;
        }
    }

    private sealed class PerformanceBehaviorOptionsFromRoot(IOptions<SyntraOptions> root) : IConfigureOptions<PerformanceBehaviorOptions>
    {
        public void Configure(PerformanceBehaviorOptions options)
        {
            options.ThresholdMilliseconds = root.Value.Performance.QueryThresholdMs;
        }
    }

    private sealed class RetryOptionsFromRoot(IOptions<SyntraOptions> root) : IConfigureOptions<RetryOptions>
    {
        public void Configure(RetryOptions options)
        {
            var r = root.Value.Retry;
            options.MaxRetries = r.MaxAttempts;
            options.BaseDelayMilliseconds = r.BaseDelayMilliseconds;
            options.BackoffMultiplier = r.BackoffMultiplier;
        }
    }

    private sealed class CacheOptionsFromRoot(IOptions<SyntraOptions> root) : IConfigureOptions<CacheOptions>
    {
        public void Configure(CacheOptions options)
        {
            options.DefaultExpiration = root.Value.Cache.DefaultExpiration;
        }
    }

    private sealed class TimeoutOptionsFromRoot(IOptions<SyntraOptions> root) : IConfigureOptions<TimeoutOptions>
    {
        public void Configure(TimeoutOptions options)
        {
            options.DefaultTimeout = root.Value.Timeout.DefaultTimeout;
        }
    }

    private sealed class CircuitBreakerOptionsFromRoot(IOptions<SyntraOptions> root) : IConfigureOptions<CircuitBreakerOptions>
    {
        public void Configure(CircuitBreakerOptions options)
        {
            var c = root.Value.CircuitBreaker;
            options.ExceptionsAllowedBeforeBreaking = c.ExceptionsAllowedBeforeBreaking;
            options.DurationOfBreak = c.DurationOfBreak;
        }
    }

    private sealed class AuthorizationBehaviorOptionsFromRoot(IOptions<SyntraOptions> root) : IConfigureOptions<AuthorizationBehaviorOptions>
    {
        public void Configure(AuthorizationBehaviorOptions options)
        {
            options.FailWhenHttpContextMissing = root.Value.Authorization.FailWhenHttpContextMissing;
        }
    }

    private sealed class IdempotencyOptionsFromRoot(IOptions<SyntraOptions> root) : IConfigureOptions<IdempotencyOptions>
    {
        public void Configure(IdempotencyOptions options)
        {
            var i = root.Value.Idempotency;
            options.CacheDuration = i.CacheDuration;
            options.CacheKeyPrefix = i.CacheKeyPrefix;
        }
    }
}
