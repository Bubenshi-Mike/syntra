namespace Syntra.Abstractions.Notifications;

/// <summary>
/// Handles a notification of type <typeparamref name="TNotification"/>.
/// Multiple handlers may be registered for the same notification type.
/// </summary>
/// <typeparam name="TNotification">
/// The notification type. Must implement <see cref="INotification"/>.
/// </typeparam>
/// <remarks>
/// <para>
/// Handlers are fire-and-forget from the publisher's perspective.
/// Any unhandled exception inside <see cref="HandleAsync"/> will be propagated
/// to the <c>INotificationPublisher</c> which decides how to handle it
/// (suppress, log, or rethrow).
/// </para>
/// <para>
/// Keep handlers focused and idempotent where possible — they may be called
/// multiple times in at-least-once delivery scenarios.
/// </para>
/// </remarks>
public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Processes the notification.
    /// </summary>
    /// <param name="notification">The notification to process. Never <c>null</c>.</param>
    /// <param name="cancellationToken">Token to observe for cooperative cancellation.</param>
    Task HandleAsync(TNotification notification, CancellationToken cancellationToken = default);
}
