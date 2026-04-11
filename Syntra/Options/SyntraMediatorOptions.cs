namespace Syntra.Options;

/// <summary>
/// Configuration for the Syntra mediator engine and notification dispatch.
/// </summary>
/// <remarks>
/// Bind via <c>IOptions&lt;SyntraMediatorOptions&gt;</c> or configure inline when calling
/// <c>AddSyntra(options =&gt; { ... })</c> in <c>Syntra.Syntra.DependencyInjection</c>.
/// </remarks>
public sealed class SyntraMediatorOptions
{
    /// <summary>
    /// The configuration section name used for <c>appsettings.json</c> binding.
    /// </summary>
    public const string SectionName = "Syntra";

    /// <summary>
    /// Chooses the default <see cref="INotificationPublisher"/> implementation
    /// registered by <c>AddSyntra()</c>. Ignored if you replace
    /// <see cref="INotificationPublisher"/> with <c>UseNotificationPublisher&lt;...&gt;()</c>.
    /// </summary>
    public NotificationDispatchStrategy NotificationDispatch { get; set; } = NotificationDispatchStrategy.Sequential;
}
