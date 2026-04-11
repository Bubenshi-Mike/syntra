using System.Diagnostics;

namespace Syntra.Diagnostics.Listeners;

/// <summary>
/// <see cref="DiagnosticListener"/> for non-OpenTelemetry consumers (opt-in).
/// </summary>
public sealed class SyntraDiagnosticsListener : DiagnosticListener
{
    /// <summary>Listener name used when subscribing.</summary>
    public new const string Name = "Syntra.Diagnostics";

    /// <summary>Singleton used by instrumentation.</summary>
    public static readonly SyntraDiagnosticsListener Instance = new();

    private SyntraDiagnosticsListener()
        : base(Name)
    {
    }

    /// <summary>Publishes a payload for a completed request pipeline.</summary>
    public void WriteRequestCompleted(string requestTypeName, bool success, double elapsedMs)
    {
        if (!IsEnabled())
            return;

        Write(
            "syntra.request.completed",
            new
            {
                RequestType = requestTypeName,
                Success = success,
                ElapsedMs = elapsedMs,
            });
    }
}
