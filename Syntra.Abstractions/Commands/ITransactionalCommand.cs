namespace Syntra.Abstractions.Commands;

/// <summary>
/// Opt-in marker for commands that must execute inside a database transaction.
/// The transaction behavior in <c>Syntra.Syntra.Behaviors</c> wraps the handler invocation
/// in a transaction, committing on success and rolling back on failure.
/// </summary>
/// <remarks>
/// Use this interface on commands that modify multiple aggregate roots or entities
/// atomically:
/// <code>
/// public sealed record TransferFundsCommand(...) : ITransactionalCommand;
/// </code>
/// Commands that only touch a single aggregate root and rely on the repository's
/// built-in transaction semantics do not need this interface.
/// </remarks>
public interface ITransactionalCommand : ICommand
{
    /// <summary>
    /// Optional isolation level hint for the transaction.
    /// <c>null</c> defers to the infrastructure default (typically <c>ReadCommitted</c>).
    /// </summary>
    public System.Data.IsolationLevel? IsolationLevel => null;
}

/// <summary>
/// Opt-in marker for value-returning commands that must execute inside a database transaction.
/// </summary>
/// <typeparam name="TResponse">The value type returned on success.</typeparam>
public interface ITransactionalCommand<TResponse> : ICommand<TResponse>
{
    /// <inheritdoc cref="ITransactionalCommand.IsolationLevel"/>
    public System.Data.IsolationLevel? IsolationLevel => null;
}
