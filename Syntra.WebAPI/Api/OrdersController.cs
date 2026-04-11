using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntra.Abstractions.Mediator;
using Syntra.WebAPI.Application.Orders;

namespace Syntra.WebAPI.Api;

/// <summary>Classic MVC-style API surface; pairs with minimal routes in <see cref="OrdersEndpoints"/>.</summary>
[ApiController]
[Route("api/orders")]
public sealed class OrdersController(ISyntraMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IResult> Place([FromBody] PlaceOrderRequest body, CancellationToken cancellationToken)
    {
        var cmd = new PlaceOrderCommand(
            body.IdempotencyKey,
            body.ProductName,
            body.Quantity,
            InitiatedBy: User.Identity?.Name,
            RequestedAt: DateTimeOffset.UtcNow,
            Reason: body.Reason);
        var result = await mediator.SendAsync(cmd, cancellationToken).ConfigureAwait(false);
        return result.ToHttp();
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IResult> GetStatus(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(new GetOrderStatusQuery(orderId), cancellationToken).ConfigureAwait(false);
        return result.ToHttp();
    }

    [HttpDelete("{orderId:guid}")]
    [Authorize(Policy = "Orders.Cancel")]
    public async Task<IResult> Cancel(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await mediator.SendAsync(new CancelOrderCommand(orderId), cancellationToken).ConfigureAwait(false);
        return result.ToHttp();
    }

    [HttpGet("{orderId:guid}/events")]
    public async Task<IResult> StreamEvents(Guid orderId, CancellationToken cancellationToken)
    {
        var stream = mediator.CreateStreamAsync(new OrderEventsStreamQuery(orderId), cancellationToken);
        return TypedResults.Ok(stream);
    }

    public sealed record PlaceOrderRequest(Guid IdempotencyKey, string ProductName, int Quantity, string? Reason);
}
