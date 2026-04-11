using System.Collections.Concurrent;

namespace Syntra.WebAPI.Application;

public sealed class OrderStore
{
    private readonly ConcurrentDictionary<Guid, OrderDto> _orders = new();

    public bool TryAdd(OrderDto order) => _orders.TryAdd(order.Id, order);

    public bool TryGet(Guid id, out OrderDto? order) => _orders.TryGetValue(id, out order);

    public bool TryRemove(Guid id) => _orders.TryRemove(id, out _);
}

public sealed record OrderDto(Guid Id, string ProductName, int Quantity, DateTimeOffset CreatedAt);

public sealed record OrderStatusDto(Guid OrderId, string ProductName, int Quantity, DateTimeOffset CreatedAt);
