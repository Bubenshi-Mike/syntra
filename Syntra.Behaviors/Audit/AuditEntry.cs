namespace Syntra.Behaviors.Audit;

/// <summary>
/// Immutable audit record emitted after a request completes.
/// </summary>
public sealed record AuditEntry(
    string RequestTypeName,
    string? InitiatedBy,
    DateTimeOffset RequestedAt,
    string? Reason,
    bool Succeeded,
    string? ErrorCode,
    string? ErrorMessage);
