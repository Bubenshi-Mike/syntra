using Syntra.Abstractions.Commands;
using Syntra.Behaviors.Transactions;

namespace Syntra.ConsoleApp.Demo;

public sealed record LogMessageCommand(Guid IdempotencyKey, string Message)
    : IIdempotentCommand, ITransactionalRequest, IAuditableRequest
{
    public string? InitiatedBy => "console-demo";

    public DateTimeOffset RequestedAt => DateTimeOffset.UtcNow;
}
