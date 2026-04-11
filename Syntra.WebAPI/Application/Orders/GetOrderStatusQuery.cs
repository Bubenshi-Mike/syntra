using Syntra.Abstractions.Queries;
using Syntra.Abstractions.Requests;

namespace Syntra.WebAPI.Application.Orders;

public sealed record GetOrderStatusQuery(Guid OrderId) : IQuery<OrderStatusDto>, ICacheableQuery<OrderStatusDto>
{
    public string CacheKey => $"order-status:{OrderId:N}";

    public TimeSpan? CacheDuration => TimeSpan.FromSeconds(45);
}
