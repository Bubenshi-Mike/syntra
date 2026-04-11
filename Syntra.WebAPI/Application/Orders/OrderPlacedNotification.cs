using Syntra.Abstractions.Notifications;

namespace Syntra.WebAPI.Application.Orders;

public sealed record OrderPlacedNotification(Guid OrderId, string ProductName, DateTimeOffset PlacedAt) : INotification;
