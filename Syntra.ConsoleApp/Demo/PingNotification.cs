using Syntra.Abstractions.Notifications;

namespace Syntra.ConsoleApp.Demo;

public sealed record PingNotification(string Source) : INotification;
