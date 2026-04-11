using Syntra.Abstractions.Commands;
using Syntra.Behaviors.Transactions;

namespace Syntra.WebAPI.Application.Orders;

/// <summary>Demonstrates idempotency, transactions, and auditing in one command.</summary>
public sealed record PlaceOrderCommand(
    Guid IdempotencyKey,
    string ProductName,
    int Quantity,
    string? InitiatedBy,
    DateTimeOffset RequestedAt,
    string? Reason)
    : IIdempotentCommand, ITransactionalRequest, IAuditableRequest;
