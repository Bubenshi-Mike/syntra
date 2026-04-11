namespace Syntra.Behaviors.Timeout;

/// <summary>
/// Options for <see cref="TimeoutBehavior{TRequest,TResponse}"/>.
/// </summary>
public sealed class TimeoutOptions
{
    /// <summary>Applied when the request does not implement <see cref="ITimeoutRequest"/> or returns <c>null</c>.</summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);
}
