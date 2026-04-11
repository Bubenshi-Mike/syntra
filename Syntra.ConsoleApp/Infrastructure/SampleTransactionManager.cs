using Syntra.Behaviors.Transactions;

namespace Syntra.ConsoleApp.Infrastructure;

internal sealed class SampleTransactionManager : ITransactionManager
{
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
