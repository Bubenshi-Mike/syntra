using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Syntra.Execution;
using Syntra.Notifications;
using Syntra.Options;

namespace Syntra.Mediator;

/// <summary>
/// Production implementation of <see cref="ISyntraMediator"/> — internal to the Syntra assembly;
/// consumers depend only on <see cref="ISyntraMediator"/>.
/// </summary>
/// <remarks>
/// <para>
/// Dispatch paths: <see cref="RequestExecutor"/> (commands/queries), <see cref="StreamExecutor"/> (streaming queries),
/// <see cref="NotificationExecutor"/> (notifications via <see cref="INotificationPublisher"/>).
/// </para>
/// <para>
/// Register via <c>AddSyntra()</c> in <c>Syntra.Syntra.DependencyInjection</c>.
/// </para>
/// </remarks>
internal sealed class SyntraMediator(
    IServiceProvider serviceProvider,
    IOptions<SyntraMediatorOptions> optionsAccessor,
    INotificationPublisher notificationPublisher,
    ILogger<SyntraMediator> logger) : ISyntraMediator
{
    private readonly IServiceProvider _serviceProvider = Guard.Against.Null(serviceProvider);
    private readonly INotificationPublisher _notificationPublisher = Guard.Against.Null(notificationPublisher);
    private readonly ILogger<SyntraMediator> _logger = Guard.Against.Null(logger);

    /// <summary>
    /// Configuration snapshot (e.g. notification defaults registered with <c>AddSyntra</c>).
    /// </summary>
    internal SyntraMediatorOptions Options => Guard.Against.Null(optionsAccessor).Value;

    /// <inheritdoc />
    /// <exception cref="Exceptions.HandlerNotFoundException">
    /// Thrown when no handler is registered for the request type.
    /// </exception>
    public async Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(request);

        _logger.LogTrace("Dispatching request {RequestType}", request.GetType().Name);

        var response = await RequestExecutor.ExecuteAsync(request, _serviceProvider, cancellationToken)
            .ConfigureAwait(false);

        _logger.LogTrace("Request {RequestType} completed", request.GetType().Name);

        return response;
    }

    /// <inheritdoc />
    /// <exception cref="Exceptions.HandlerNotFoundException">
    /// Thrown lazily when enumeration starts if no stream handler is registered.
    /// </exception>
    public IAsyncEnumerable<TResponse> CreateStreamAsync<TResponse>(
        IStreamQuery<TResponse> query,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(query);

        _logger.LogTrace("Dispatching stream query {QueryType}", query.GetType().Name);

        return StreamExecutor.ExecuteAsync(query, _serviceProvider, cancellationToken);
    }

    /// <inheritdoc />
    public async Task PublishAsync<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        Guard.Against.Null(notification);

        _logger.LogTrace("Publishing notification {NotificationType}", notification.GetType().Name);

        await NotificationExecutor.PublishAsync(notification, _notificationPublisher, _serviceProvider, cancellationToken)
            .ConfigureAwait(false);

        _logger.LogTrace("Notification {NotificationType} published", notification.GetType().Name);
    }
}
