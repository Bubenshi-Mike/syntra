using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Results;

namespace Syntra.WebAPI.Application.Orders;

public sealed class CancelOrderCommandHandler(OrderStore store, ILogger<CancelOrderCommandHandler> logger)
    : ICommandHandler<CancelOrderCommand>
{
    public Task<Result> HandleAsync(CancelOrderCommand request, CancellationToken cancellationToken = default)
    {
        if (!store.TryRemove(request.OrderId))
            return Task.FromResult(Result.Failure(Error.NotFound("Order.Missing", "Order not found.")));

        logger.LogWarning("Order {OrderId} cancelled", request.OrderId);
        return Task.FromResult(Result.Success());
    }
}
