namespace Syntra.Notifications;

/// <summary>
/// An <see cref="INotificationPublisher"/> that starts all handler tasks concurrently
/// and returns immediately without awaiting their completion (fire-and-forget).
/// </summary>
/// <remarks>
/// <para>
/// Handler exceptions are silently swallowed because no one is awaiting the tasks.
/// This strategy is appropriate for truly fire-and-forget side-effects (telemetry,
/// non-critical cache invalidations, etc.) where the caller must not block on delivery.
/// </para>
/// <para>
/// In hosted environments, prefer registering a background service rather than using
/// this publisher for work that must complete before the application shuts down.
/// </para>
/// <para>
/// <strong>Warning:</strong> Unobserved task exceptions are raised on
/// <see cref="TaskScheduler.UnobservedTaskException"/>. Register a handler on that
/// event if you need fire-and-forget exceptions to be logged.
/// </para>
/// </remarks>
public sealed class ParallelNoWaitNotificationPublisher : INotificationPublisher
{
    /// <inheritdoc />
    public Task PublishAsync<TNotification>(
        IEnumerable<INotificationHandler<TNotification>> handlers,
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        foreach (var handler in handlers)
        {
            _ = handler.HandleAsync(notification, cancellationToken);
        }

        return Task.CompletedTask;
    }
}
