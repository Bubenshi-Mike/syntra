using Syntra.Behaviors.Audit;

namespace Syntra.ConsoleApp.Infrastructure;

internal sealed class SampleAuditWriter(ILogger<SampleAuditWriter> logger) : IAuditWriter
{
    public Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[AUDIT] {Type} success={Ok}",
            entry.RequestTypeName,
            entry.Succeeded);
        return Task.CompletedTask;
    }
}
