using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Mediator;
using Syntra.Abstractions.Results;

namespace Syntra.WebAPI.Application.Orders;

public sealed class PlaceOrderCommandHandler(
    OrderStore store,
    ISyntraMediator mediator,
    ILogger<PlaceOrderCommandHandler> logger) : ICommandHandler<PlaceOrderCommand>
{
    public async Task<Result> HandleAsync(PlaceOrderCommand request, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        var dto = new OrderDto(id, request.ProductName, request.Quantity, DateTimeOffset.UtcNow);

        if (!store.TryAdd(dto))
            return Result.Failure(Error.Conflict("Order.Store", "Could not persist order (unexpected)."));

        logger.LogInformation("Order {OrderId} created for product {Product}", id, request.ProductName);

        await mediator
            .PublishAsync(new OrderPlacedNotification(id, request.ProductName, dto.CreatedAt), cancellationToken)
            .ConfigureAwait(false);

        return Result.Success();
    }
}
