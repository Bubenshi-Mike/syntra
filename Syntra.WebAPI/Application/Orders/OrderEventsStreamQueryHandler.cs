using Syntra.Abstractions.Handlers;

namespace Syntra.WebAPI.Application.Orders;

public sealed class OrderEventsStreamQueryHandler(OrderStore store) : IStreamQueryHandler<OrderEventsStreamQuery, OrderStreamEvent>
{
    public async IAsyncEnumerable<OrderStreamEvent> HandleAsync(
        OrderEventsStreamQuery query,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!store.TryGet(query.OrderId, out var order) || order is null)
            yield break;

        await Task.Yield();
        yield return new OrderStreamEvent("validated", DateTimeOffset.UtcNow);
        await Task.Delay(80, cancellationToken).ConfigureAwait(false);
        yield return new OrderStreamEvent("packed", DateTimeOffset.UtcNow);
        await Task.Delay(80, cancellationToken).ConfigureAwait(false);
        yield return new OrderStreamEvent("shipped", order.CreatedAt.AddMinutes(1));
    }
}
