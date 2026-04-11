using Syntra.Abstractions.Notifications;

namespace Syntra.Abstractions.Mediator;

/// <summary>
/// Strategy contract for fanning a notification out to its registered handlers.
/// </summary>
/// <remarks>
/// <para>
/// Decouple delivery semantics from the mediator itself.
/// The default implementation dispatches handlers sequentially.
/// Replace with a parallel or fire-and-forget implementation by registering
/// a custom <see cref="INotificationPublisher"/> in the DI container.
/// </para>
/// <para>
/// Built-in strategies provided by <c>Syntra.Syntra.DependencyInjection</c>:
/// <list type="bullet">
///   <item><term>Sequential</term><description>Invokes handlers one by one in registration order. Any failure stops the chain.</description></item>
///   <item><term>ParallelNoWait</term><description>Invokes all handlers concurrently and does not await completion.</description></item>
///   <item><term>ParallelWhenAll</term><description>Invokes all handlers concurrently and awaits all of them.</description></item>
/// </list>
/// </para>
/// </remarks>
public interface INotificationPublisher
{
    /// <summary>
    /// Dispatches <paramref name="notification"/> to each handler in <paramref name="handlers"/>.
    /// </summary>
    /// <typeparam name="TNotification">The notification type.</typeparam>
    /// <param name="handlers">
    /// The handlers registered for <typeparamref name="TNotification"/>.
    /// May be empty; implementations must treat that as a no-op.
    /// </param>
    /// <param name="notification">The notification to deliver.</param>
    /// <param name="cancellationToken">Token to observe for cooperative cancellation.</param>
    Task PublishAsync<TNotification>(
        IEnumerable<INotificationHandler<TNotification>> handlers,
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification;
}
