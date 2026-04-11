namespace Syntra.Internal;

/// <summary>
/// Non-generic wrapper that fans a notification out to all its registered handlers
/// using the configured <see cref="INotificationPublisher"/> strategy.
/// </summary>
internal interface INotificationHandlerWrapper
{
    /// <summary>
    /// Publishes the notification to all registered handlers.
    /// </summary>
    /// <param name="notification">The notification object (cast internally to the concrete type).</param>
    /// <param name="publisher">The strategy used to invoke the handlers.</param>
    /// <param name="serviceProvider">Scoped DI container for resolving notification handlers.</param>
    /// <param name="cancellationToken">Token to observe for cooperative cancellation.</param>
    Task PublishAsync(
        object notification,
        INotificationPublisher publisher,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}
