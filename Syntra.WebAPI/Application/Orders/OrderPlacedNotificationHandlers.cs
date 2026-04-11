using Syntra.Abstractions.Notifications;

namespace Syntra.WebAPI.Application.Orders;

/// <summary>Two handlers demonstrate notification fan-out.</summary>
public sealed class OrderPlacedAuditLogHandler(ILogger<OrderPlacedAuditLogHandler> logger) : INotificationHandler<OrderPlacedNotification>
{
    public Task HandleAsync(OrderPlacedNotification notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[NOTIFICATION] Order {OrderId} placed — audit trail hook", notification.OrderId);
        return Task.CompletedTask;
    }
}

public sealed class OrderPlacedMetricsHandler(ILogger<OrderPlacedMetricsHandler> logger) : INotificationHandler<OrderPlacedNotification>
{
    public Task HandleAsync(OrderPlacedNotification notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[NOTIFICATION] Order {OrderId} — metrics / side-effects hook", notification.OrderId);
        return Task.CompletedTask;
    }
}
