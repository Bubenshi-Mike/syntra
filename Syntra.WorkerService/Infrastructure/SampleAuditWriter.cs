using Syntra.Behaviors.Audit;

namespace Syntra.WorkerService.Infrastructure;

internal sealed class SampleAuditWriter(ILogger<SampleAuditWriter> logger) : IAuditWriter
{
    public Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("[AUDIT] {Type}", entry.RequestTypeName);
        return Task.CompletedTask;
    }
}
