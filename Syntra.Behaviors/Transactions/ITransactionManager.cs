namespace Syntra.Behaviors.Transactions;

/// <summary>
/// Infrastructure contract for unit-of-work boundaries (EF Core, Dapper, etc.).
/// </summary>
public interface ITransactionManager
{
    /// <summary>Begins a transaction on the underlying connection/scope.</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Commits the active transaction.</summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Rolls back the active transaction.</summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
