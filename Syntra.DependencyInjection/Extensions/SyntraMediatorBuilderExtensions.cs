using Syntra.DependencyInjection.Registration;

namespace Syntra.DependencyInjection.Extensions;

/// <summary>
/// Fluent shortcuts on <see cref="SyntraBuilder"/> for handler registration and scanning.
/// </summary>
public static class SyntraMediatorBuilderExtensions
{
    /// <summary>Registers an open-generic pipeline behavior.</summary>
    public static SyntraBuilder AddOpenGenericBehavior(
        this SyntraBuilder builder,
        Type openGenericBehaviorType,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        => builder.AddBehavior(openGenericBehaviorType, lifetime);
}
