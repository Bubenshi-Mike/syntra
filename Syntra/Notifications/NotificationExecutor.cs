using Ardalis.GuardClauses;
using Syntra.Caching;

namespace Syntra.Notifications;

/// <summary>
/// Fan-out entry point for notifications: resolves the cached per-notification-type bridge
/// and delegates to <see cref="INotificationPublisher"/>.
/// </summary>
internal static class NotificationExecutor
{
    /// <summary>
    /// Publishes <paramref name="notification"/> using <paramref name="publisher"/>.
    /// </summary>
    public static Task PublishAsync(
        object notification,
        INotificationPublisher publisher,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification);
        Guard.Against.Null(publisher);
        Guard.Against.Null(serviceProvider);

        var wrapper = NotificationDelegateCache.GetOrCreate(notification.GetType());
        return wrapper.PublishAsync(notification, publisher, serviceProvider, cancellationToken);
    }
}
