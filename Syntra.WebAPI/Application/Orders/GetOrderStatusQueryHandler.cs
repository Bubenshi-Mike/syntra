using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Results;

namespace Syntra.WebAPI.Application.Orders;

public sealed class GetOrderStatusQueryHandler(OrderStore store) : IQueryHandler<GetOrderStatusQuery, OrderStatusDto>
{
    public Task<Result<OrderStatusDto>> HandleAsync(GetOrderStatusQuery request, CancellationToken cancellationToken = default)
    {
        if (!store.TryGet(request.OrderId, out var dto) || dto is null)
            return Task.FromResult(Result.Failure<OrderStatusDto>(Error.NotFound("Order.Missing", "Order not found.")));

        var status = new OrderStatusDto(dto.Id, dto.ProductName, dto.Quantity, dto.CreatedAt);
        return Task.FromResult(Result.Success(status));
    }
}
