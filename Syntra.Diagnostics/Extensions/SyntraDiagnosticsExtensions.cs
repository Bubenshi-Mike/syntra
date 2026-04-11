using Microsoft.Extensions.DependencyInjection.Extensions;
using Syntra.Diagnostics.Listeners;
using Syntra.Diagnostics.Metrics;
using Syntra.Diagnostics.Tracing;

namespace Syntra.Diagnostics.Extensions;

/// <summary>
/// Opt-in registration for Syntra diagnostics (tracing, metrics, <see cref="System.Diagnostics.DiagnosticListener"/>).
/// </summary>
public static class SyntraDiagnosticsExtensions
{
    /// <summary>
    /// Registers <see cref="TracingBehavior{TRequest,TResponse}"/> and <see cref="RequestMetricsBehavior{TRequest,TResponse}"/>
    /// as pipeline behaviors, and the <see cref="SyntraDiagnosticsListener"/> singleton.
    /// </summary>
    public static IServiceCollection AddSyntraDiagnostics(this IServiceCollection services)
    {
        services.TryAddSingleton(_ => SyntraDiagnosticsListener.Instance);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TracingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestMetricsBehavior<,>));
        return services;
    }
}
