namespace Syntra.Behaviors.Performance;

/// <summary>
/// Options for <see cref="PerformanceBehavior{TRequest,TResponse}"/>.
/// </summary>
public sealed class PerformanceBehaviorOptions
{
    /// <summary>
    /// Elapsed milliseconds above which a warning is emitted. Default: 500.
    /// </summary>
    public long ThresholdMilliseconds { get; set; } = 500;
}
