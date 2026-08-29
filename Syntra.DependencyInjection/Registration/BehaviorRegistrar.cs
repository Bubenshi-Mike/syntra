namespace Syntra.DependencyInjection.Registration;

/// <summary>
/// Applies <see cref="BehaviorRegistrationBuilder"/> steps to the service collection in order.
/// </summary>
public static class BehaviorRegistrar
{
    /// <summary>Runs all queued behavior registrations (outermost first).</summary>
    public static void Apply(IServiceCollection services, BehaviorRegistrationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(builder);

        foreach (var step in builder.Steps)
        {
            step(services);
        }
    }
}
