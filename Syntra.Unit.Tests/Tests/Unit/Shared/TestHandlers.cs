using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Notifications;
using Syntra.Abstractions.Results;

namespace Syntra.Unit.Tests.Tests.Unit.Shared;

internal sealed class UnitPingQueryHandler : IQueryHandler<UnitPingQuery, int>
{
    public Task<Result<int>> HandleAsync(UnitPingQuery request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success(42));
}

internal sealed class UnitEmptyCommandHandler : ICommandHandler<UnitEmptyCommand>
{
    public Task<Result> HandleAsync(UnitEmptyCommand request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success());
}

internal sealed class UnitStreamNumbersQueryHandler : IStreamQueryHandler<UnitStreamNumbersQuery, int>
{
    public async IAsyncEnumerable<int> HandleAsync(
        UnitStreamNumbersQuery query,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        yield return 1;
        yield return 2;
    }
}

internal sealed class UnitTestNotificationHandler : INotificationHandler<UnitTestNotification>
{
    public Task HandleAsync(UnitTestNotification notification, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
