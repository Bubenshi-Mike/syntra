using Syntra.Abstractions.Notifications;

namespace Syntra.Abstractions.Mediator;

/// <summary>
/// Central mediator contract. Resolves handlers and dispatches requests, queries,
/// streams, and notifications.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ISyntraMediator"/> is the single entry-point into the Syntra pipeline.
/// Consumers take it as a constructor dependency and use it to dispatch work
/// without coupling to handler implementations.
/// </para>
/// <para>
/// The three dispatch paths:
/// <list type="table">
///   <listheader>
///     <term>Method</term>
///     <description>Use case</description>
///   </listheader>
///   <item>
///     <term><see cref="SendAsync{TResponse}"/></term>
///     <description>Commands and queries — exactly one handler, returns a <c>Result</c>.</description>
///   </item>
///   <item>
///     <term><see cref="CreateStreamAsync{TResponse}"/></term>
///     <description>Streaming queries — exactly one handler, returns an async sequence.</description>
///   </item>
///   <item>
///     <term><see cref="PublishAsync{TNotification}"/></term>
///     <description>Notifications — zero or more handlers, no return value.</description>
///   </item>
/// </list>
/// </para>
/// </remarks>
public interface ISyntraMediator
{
    /// <summary>
    /// Dispatches <paramref name="request"/> through the pipeline to its registered handler
    /// and returns the response.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The response type declared on the request (e.g. <c>Result</c> or <c>Result&lt;Guid&gt;</c>).
    /// </typeparam>
    /// <param name="request">The command or query to dispatch. Must not be <c>null</c>.</param>
    /// <param name="cancellationToken">Token to observe for cooperative cancellation.</param>
    /// <returns>A task that resolves to the handler's response.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no handler is registered for <paramref name="request"/>'s type.
    /// </exception>
    public Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches <paramref name="query"/> to its registered stream handler and returns
    /// a lazy async sequence of items.
    /// </summary>
    /// <typeparam name="TResponse">The type of each item in the stream.</typeparam>
    /// <param name="query">The streaming query to dispatch. Must not be <c>null</c>.</param>
    /// <param name="cancellationToken">Token to observe for cooperative cancellation.</param>
    /// <returns>
    /// An <see cref="IAsyncEnumerable{T}"/> of <typeparamref name="TResponse"/>.
    /// The sequence is lazy — handler execution does not begin until the caller
    /// starts iterating with <c>await foreach</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no handler is registered for <paramref name="query"/>'s type.
    /// </exception>
    public IAsyncEnumerable<TResponse> CreateStreamAsync<TResponse>(
        IStreamQuery<TResponse> query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes <paramref name="notification"/> to all registered handlers
    /// using the configured <see cref="INotificationPublisher"/> strategy.
    /// </summary>
    /// <typeparam name="TNotification">The notification type.</typeparam>
    /// <param name="notification">The notification to publish. Must not be <c>null</c>.</param>
    /// <param name="cancellationToken">Token to observe for cooperative cancellation.</param>
    /// <returns>
    /// A task that completes when all handlers have been invoked
    /// (exact semantics depend on the configured <see cref="INotificationPublisher"/>).
    /// </returns>
    public Task PublishAsync<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification;
}
