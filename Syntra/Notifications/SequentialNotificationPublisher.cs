namespace Syntra.Notifications;

/// <summary>
/// Default <see cref="INotificationPublisher"/> that invokes notification handlers one by one,
/// in registration order. If a handler throws, the exception propagates immediately
/// and remaining handlers are not called.
/// </summary>
/// <remarks>
/// This is the default strategy registered by <c>AddSyntra()</c> in
/// <c>Syntra.Syntra.DependencyInjection</c>. Replace with <see cref="ParallelWhenAllNotificationPublisher"/>
/// or a custom implementation for different delivery semantics.
/// </remarks>
public sealed class SequentialNotificationPublisher : INotificationPublisher
{
    /// <inheritdoc />
    public async Task PublishAsync<TNotification>(
        IEnumerable<INotificationHandler<TNotification>> handlers,
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(notification, cancellationToken).ConfigureAwait(false);
        }
    }
}
