namespace Syntra.Abstractions.Commands;

/// <summary>
/// Opt-in contract for requests that must be captured in an audit trail.
/// The audit behavior in <c>Syntra.Syntra.Behaviors</c> records who triggered the request,
/// when, and — after the handler completes — whether it succeeded or failed.
/// </summary>
/// <remarks>
/// Apply to commands (and occasionally queries) where regulatory, compliance,
/// or security requirements mandate an immutable audit log:
/// <code>
/// public sealed record DeleteUserCommand(
///     Guid UserId,
///     string InitiatedBy,
///     DateTimeOffset RequestedAt) : ICommand, IAuditableRequest;
/// </code>
/// </remarks>
public interface IAuditableRequest
{
    /// <summary>
    /// The identity (user ID, service account name, etc.) that initiated the request.
    /// <c>null</c> indicates an anonymous or system-initiated action.
    /// </summary>
    string? InitiatedBy { get; }

    /// <summary>
    /// The UTC timestamp at which the request was created/submitted.
    /// </summary>
    DateTimeOffset RequestedAt { get; }

    /// <summary>
    /// Optional free-form description of the business reason for this action.
    /// Useful for compliance scenarios where a human-readable justification is required.
    /// </summary>
    string? Reason => null;
}
