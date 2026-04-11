namespace Syntra.Notifications;

/// <summary>
/// An <see cref="INotificationPublisher"/> that invokes all handlers concurrently and
/// awaits all of them before returning. All handler tasks are started simultaneously;
/// the publisher waits for the last one to finish.
/// </summary>
/// <remarks>
/// <para>
/// If one or more handlers fault, all exceptions are collected into an
/// <see cref="AggregateException"/> propagated by <c>Task.WhenAll</c>.
/// </para>
/// <para>
/// Use this strategy when handler independence is guaranteed and you want to minimize
/// the total latency of notification delivery. Avoid if handlers share state or if
/// partial-failure isolation is required.
/// </para>
/// </remarks>
public sealed class ParallelWhenAllNotificationPublisher : INotificationPublisher
{
    /// <inheritdoc />
    public Task PublishAsync<TNotification>(
        IEnumerable<INotificationHandler<TNotification>> handlers,
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var tasks = handlers
            .Select(h => h.HandleAsync(notification, cancellationToken))
            .ToArray();

        return tasks.Length == 0 ? Task.CompletedTask : Task.WhenAll(tasks);
    }
}
