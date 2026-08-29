namespace Syntra.Behaviors.Timeout;

/// <summary>
/// Optional per-request timeout override; when <see cref="Timeout"/> is <c>null</c>,
/// <see cref="TimeoutOptions.DefaultTimeout"/> is used.
/// </summary>
public interface ITimeoutRequest
{
    /// <summary>Per-request deadline, or <c>null</c> to use global default.</summary>
    public TimeSpan? Timeout { get; }
}
