using Syntra.Abstractions.Notifications;

namespace Syntra.WorkerService.Demo;

public sealed class HeartbeatLogHandler(ILogger<HeartbeatLogHandler> logger) : INotificationHandler<HeartbeatNotification>
{
    public Task HandleAsync(HeartbeatNotification notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[NOTIFICATION] Heartbeat tick {Tick} at {At:O}", notification.Tick, notification.At);
        return Task.CompletedTask;
    }
}

public sealed class HeartbeatMetricsHandler(ILogger<HeartbeatMetricsHandler> logger) : INotificationHandler<HeartbeatNotification>
{
    public Task HandleAsync(HeartbeatNotification notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[NOTIFICATION] Secondary observer for tick {Tick}", notification.Tick);
        return Task.CompletedTask;
    }
}
