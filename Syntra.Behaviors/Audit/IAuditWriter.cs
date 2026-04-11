namespace Syntra.Behaviors.Audit;

/// <summary>
/// Infrastructure hook that persists <see cref="AuditEntry"/> records (database, event bus, etc.).
/// </summary>
public interface IAuditWriter
{
    /// <summary>Persists <paramref name="entry"/>.</summary>
    Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}
