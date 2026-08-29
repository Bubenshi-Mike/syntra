namespace Syntra.Abstractions.Commands;

/// <summary>
/// Opt-in contract for commands that must be deduplicated by an idempotency key.
/// The idempotency behavior in <c>Syntra.Syntra.Behaviors</c> inspects this interface
/// and returns the cached response on duplicate submissions.
/// </summary>
/// <remarks>
/// Implement on commands that could be replayed (e.g. from message queues,
/// retried HTTP requests, or outbox patterns):
/// <code>
/// public sealed record CreateOrderCommand(Guid IdempotencyKey, ...) : IIdempotentCommand;
/// </code>
/// </remarks>
public interface IIdempotentCommand : ICommand
{
    /// <summary>
    /// A unique key that identifies this logical operation.
    /// Two commands with the same key represent the same intent and
    /// the second should be a no-op returning the original result.
    /// </summary>
    public Guid IdempotencyKey { get; }
}

/// <summary>
/// Opt-in contract for value-returning commands that must be deduplicated.
/// </summary>
/// <typeparam name="TResponse">The value type returned on success.</typeparam>
public interface IIdempotentCommand<TResponse> : ICommand<TResponse>
{
    /// <inheritdoc cref="IIdempotentCommand.IdempotencyKey"/>
    public Guid IdempotencyKey { get; }
}
