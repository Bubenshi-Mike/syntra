using Syntra.Abstractions.Mediator;
using Syntra.WebAPI.Application.Orders;

namespace Syntra.WebAPI.Api;

/// <summary>Minimal API group mirroring <see cref="OrdersController"/> — same mediator pipeline.</summary>
public static class OrdersEndpoints
{
    public static void MapOrdersMinimalApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/orders-minimal").WithTags("Orders (Minimal API)");

        group.MapPost(
                "/",
                async (
                    PlaceOrderRequest body,
                    HttpContext http,
                    ISyntraMediator mediator,
                    CancellationToken cancellationToken) =>
                {
                    var cmd = new PlaceOrderCommand(
                        body.IdempotencyKey,
                        body.ProductName,
                        body.Quantity,
                        InitiatedBy: http.User.Identity?.Name,
                        RequestedAt: DateTimeOffset.UtcNow,
                        Reason: body.Reason);
                    var result = await mediator.SendAsync(cmd, cancellationToken).ConfigureAwait(false);
                    return result.ToHttp();
                })
            .WithName("MinimalPlaceOrder")
            .WithSummary("Place order (minimal API)")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapGet(
                "/{orderId:guid}",
                async (Guid orderId, ISyntraMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.SendAsync(new GetOrderStatusQuery(orderId), cancellationToken)
                        .ConfigureAwait(false);
                    return result.ToHttp();
                })
            .WithName("MinimalGetOrder");

        group.MapDelete(
                "/{orderId:guid}",
                async (Guid orderId, ISyntraMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.SendAsync(new CancelOrderCommand(orderId), cancellationToken)
                        .ConfigureAwait(false);
                    return result.ToHttp();
                })
            .RequireAuthorization("Orders.Cancel")
            .WithName("MinimalCancelOrder");

        group.MapGet(
                "/{orderId:guid}/events",
                (Guid orderId, ISyntraMediator mediator, CancellationToken cancellationToken) =>
                {
                    var stream = mediator.CreateStreamAsync(new OrderEventsStreamQuery(orderId), cancellationToken);
                    return TypedResults.Ok(stream);
                })
            .WithName("MinimalOrderEventStream");
    }

    public sealed record PlaceOrderRequest(Guid IdempotencyKey, string ProductName, int Quantity, string? Reason);
}
