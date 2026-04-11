using Syntra.Behaviors.Transactions;

namespace Syntra.WebAPI.Infrastructure;

/// <summary>No-op unit-of-work for the sample. Kept internal so Scrutor does not auto-register it.</summary>
internal sealed class SampleTransactionManager(ILogger<SampleTransactionManager> logger) : ITransactionManager
{
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Transaction begin (sample no-op)");
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Transaction commit (sample no-op)");
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Transaction rollback (sample no-op)");
        return Task.CompletedTask;
    }
}
