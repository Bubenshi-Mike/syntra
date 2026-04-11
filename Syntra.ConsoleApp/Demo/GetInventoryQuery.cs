using Syntra.Abstractions.Queries;
using Syntra.Abstractions.Requests;

namespace Syntra.ConsoleApp.Demo;

public sealed record GetInventoryQuery(string Sku) : IQuery<int>, ICacheableQuery<int>
{
    public string CacheKey => $"inventory:{Sku}";

    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(5);
}
