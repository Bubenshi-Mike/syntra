namespace Syntra.Options;

/// <summary>
/// Selects how <see cref="ISyntraMediator.PublishAsync{TNotification}"/> fans out work
/// to notification handlers when using the default DI registration from
/// <c>AddSyntra()</c>.
/// </summary>
public enum NotificationDispatchStrategy
{
    /// <summary>
    /// Handlers run one after another in registration order.
    /// </summary>
    Sequential = 0,

    /// <summary>
    /// All handlers start concurrently; the publisher awaits every task (<see cref="Task.WhenAll"/>).
    /// </summary>
    ParallelWhenAll = 1,

    /// <summary>
    /// All handlers start concurrently; the publisher returns immediately without awaiting.
    /// </summary>
    ParallelFireAndForget = 2,
}
