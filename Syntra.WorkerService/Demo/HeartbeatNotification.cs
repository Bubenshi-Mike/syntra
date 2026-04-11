using Syntra.Abstractions.Notifications;

namespace Syntra.WorkerService.Demo;

public sealed record HeartbeatNotification(DateTimeOffset At, int Tick) : INotification;
