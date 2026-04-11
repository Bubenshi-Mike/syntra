using Syntra.Behaviors.Audit;

namespace Syntra.WebAPI.Infrastructure;

/// <summary>Sample audit sink: structured logs. Kept internal so Scrutor does not auto-register it (we use <c>UseAuditWriter</c>).</summary>
internal sealed class SampleAuditWriter(ILogger<SampleAuditWriter> logger) : IAuditWriter
{
    public Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[AUDIT] {RequestType} by {InitiatedBy} at {RequestedAt} success={Succeeded} code={ErrorCode} reason={Reason}",
            entry.RequestTypeName,
            entry.InitiatedBy ?? "(anonymous)",
            entry.RequestedAt,
            entry.Succeeded,
            entry.ErrorCode,
            entry.Reason ?? "(none)");
        return Task.CompletedTask;
    }
}
