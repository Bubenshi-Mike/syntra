using Syntra.Abstractions.Requests;

namespace Syntra.WebAPI.Application.Orders;

public sealed record OrderStreamEvent(string Phase, DateTimeOffset At);

public sealed record OrderEventsStreamQuery(Guid OrderId) : IStreamQuery<OrderStreamEvent>;
