using Syntra.Internal;

namespace Syntra.Notifications;

/// <summary>
/// Resolves all <see cref="INotificationHandler{TNotification}"/> registrations and delegates
/// fan-out to <see cref="INotificationPublisher"/>.
/// </summary>
/// <typeparam name="TNotification">The concrete notification type.</typeparam>
internal sealed class NotificationPipelineExecutor<TNotification> : INotificationHandlerWrapper
    where TNotification : INotification
{
    /// <inheritdoc />
    public Task PublishAsync(
        object notification,
        INotificationPublisher publisher,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var typed = (TNotification)notification;
        var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>();
        return publisher.PublishAsync(handlers, typed, cancellationToken);
    }
}
