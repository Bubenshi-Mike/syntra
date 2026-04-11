using Syntra.Abstractions.Notifications;

namespace Syntra.ConsoleApp.Demo;

public sealed class PingNotificationHandlerA(ILogger<PingNotificationHandlerA> logger) : INotificationHandler<PingNotification>
{
    public Task HandleAsync(PingNotification notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[NOTIFICATION-A] from {Source}", notification.Source);
        return Task.CompletedTask;
    }
}

public sealed class PingNotificationHandlerB(ILogger<PingNotificationHandlerB> logger) : INotificationHandler<PingNotification>
{
    public Task HandleAsync(PingNotification notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[NOTIFICATION-B] fan-out peer for {Source}", notification.Source);
        return Task.CompletedTask;
    }
}
