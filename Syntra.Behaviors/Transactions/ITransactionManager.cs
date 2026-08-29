namespace Syntra.Behaviors.Transactions;

/// <summary>
/// Infrastructure contract for unit-of-work boundaries (EF Core, Dapper, etc.).
/// </summary>
public interface ITransactionManager
{
    /// <summary>Begins a transaction on the underlying connection/scope.</summary>
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Commits the active transaction.</summary>
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Rolls back the active transaction.</summary>
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
