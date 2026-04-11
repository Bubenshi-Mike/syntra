namespace Syntra.Diagnostics.Telemetry;

/// <summary>Well-known activity names for Syntra tracing.</summary>
public static class SyntraActivityNames
{
    /// <summary>Root span for a mediator Send pipeline.</summary>
    public const string MediatorRequest = "syntra.mediator.request";

    /// <summary>Span for a single pipeline behavior.</summary>
    public const string MediatorBehavior = "syntra.mediator.behavior";
}
