using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Results;

namespace Syntra.ConsoleApp.Demo;

public sealed class GetInventoryQueryHandler : IQueryHandler<GetInventoryQuery, int>
{
    public Task<Result<int>> HandleAsync(GetInventoryQuery request, CancellationToken cancellationToken = default)
    {
        var qty = Random.Shared.Next(1, 100);
        return Task.FromResult(Result.Success(qty));
    }
}
