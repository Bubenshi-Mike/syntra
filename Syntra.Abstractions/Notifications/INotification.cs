namespace Syntra.Abstractions.Notifications;

/// <summary>
/// Marker interface for a notification — an event that has already happened
/// and may be observed by zero or more independent handlers.
/// </summary>
/// <remarks>
/// <para>
/// Notifications model the "fan-out" or "pub-sub" pattern.
/// Unlike requests (which expect exactly one handler), a notification can have
/// any number of handlers including none.
/// </para>
/// <para>
/// Notifications must not carry return values. If you need a response, use a
/// request/handler pair instead.
/// </para>
/// <para>
/// Implementations should be immutable value objects that capture the facts
/// of what happened, e.g.:
/// <code>
/// public sealed record OrderPlacedNotification(Guid OrderId, DateTimeOffset PlacedAt)
///     : INotification;
/// </code>
/// </para>
/// </remarks>
public interface INotification { }
