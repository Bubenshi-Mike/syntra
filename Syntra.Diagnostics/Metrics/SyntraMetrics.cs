using System.Diagnostics.Metrics;

namespace Syntra.Diagnostics.Metrics;

/// <summary>
/// OpenTelemetry-compatible <see cref="Meter"/> instruments for Syntra (opt-in).
/// </summary>
public static class SyntraMetrics
{
    /// <summary>Meter name for OTLP / Prometheus exporters.</summary>
    public const string MeterName = "Syntra";

    /// <summary>Singleton meter instance.</summary>
    public static readonly Meter Meter = new(MeterName, "1.0.0");

    /// <summary>Histogram of request duration in milliseconds.</summary>
    public static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>(
        "syntra.requests.duration",
        unit: "ms",
        description: "Mediator request pipeline duration");

    /// <summary>Total requests dispatched.</summary>
    public static readonly Counter<long> RequestTotal = Meter.CreateCounter<long>(
        "syntra.requests.total",
        description: "Total mediator requests");

    /// <summary>Failed requests (domain or unexpected).</summary>
    public static readonly Counter<long> RequestFailures = Meter.CreateCounter<long>(
        "syntra.requests.failures",
        description: "Failed mediator requests");

    /// <summary>Notifications published.</summary>
    public static readonly Counter<long> NotificationsDispatched = Meter.CreateCounter<long>(
        "syntra.notifications.dispatched",
        description: "Notifications published");

    /// <summary>Cache hits (when instrumentation is present).</summary>
    public static readonly Counter<long> CacheHits = Meter.CreateCounter<long>(
        "syntra.cache.hits",
        description: "Cache hits");

    /// <summary>Cache misses.</summary>
    public static readonly Counter<long> CacheMisses = Meter.CreateCounter<long>(
        "syntra.cache.misses",
        description: "Cache misses");
}
