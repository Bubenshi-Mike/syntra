using Syntra.Behaviors.Audit;
using Syntra.Behaviors.Transactions;

namespace Syntra.DependencyInjection.Extensions;

/// <summary>
/// Infrastructure port registration helpers.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers <typeparamref name="T"/> as <see cref="ITransactionManager"/> (typically scoped).</summary>
    public static IServiceCollection UseTransactionManager<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where T : class, ITransactionManager
    {
        services.Add(new ServiceDescriptor(typeof(ITransactionManager), typeof(T), lifetime));
        return services;
    }

    /// <summary>Registers <typeparamref name="T"/> as <see cref="IAuditWriter"/> (typically singleton).</summary>
    public static IServiceCollection UseAuditWriter<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where T : class, IAuditWriter
    {
        services.Add(new ServiceDescriptor(typeof(IAuditWriter), typeof(T), lifetime));
        return services;
    }
}
