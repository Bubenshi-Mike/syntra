using System.Diagnostics;

namespace Syntra.Diagnostics.Telemetry;

/// <summary>
/// Static holder for the Syntra <see cref="ActivitySource"/> (opt-in tracing).
/// </summary>
public static class DiagnosticsActivitySource
{
    /// <summary>Listener name (OpenTelemetry convention).</summary>
    public const string Name = "Syntra";

    /// <summary>Subscribe via <c>ActivitySource.AddActivityListener</c> or OpenTelemetry.</summary>
    public static readonly ActivitySource Instance = new(Name, "1.0.0");
}
