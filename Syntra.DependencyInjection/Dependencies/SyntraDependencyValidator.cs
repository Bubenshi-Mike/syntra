using Syntra.Behaviors.Audit;
using Syntra.Behaviors.Transactions;

namespace Syntra.DependencyInjection.Dependencies;

/// <summary>
/// Best-effort validation of required infrastructure services for enabled behaviors.
/// </summary>
public static class SyntraDependencyValidator
{
    /// <summary>
    /// Throws <see cref="InvalidOperationException"/> when a behavior is registered but its port is missing.
    /// </summary>
    public static void Validate(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var descriptors = services.ToList();

        if (HasOpenGenericBehavior(descriptors, typeof(AuditBehavior<,>)) && !HasService<IAuditWriter>(descriptors))
        {
            throw new InvalidOperationException(
                "Audit pipeline behavior is registered but no implementation of " + nameof(IAuditWriter) +
                " was found. Register an audit writer or remove AddAudit() from the behavior pipeline.");
        }

        if (HasOpenGenericBehavior(descriptors, typeof(TransactionBehavior<,>)) && !HasService<ITransactionManager>(descriptors))
        {
            throw new InvalidOperationException(
                "Transaction pipeline behavior is registered but no implementation of " + nameof(ITransactionManager) +
                " was found. Call services.UseTransactionManager<T>() or register " + nameof(ITransactionManager) + " manually.");
        }
    }

    private static bool HasService<TService>(List<ServiceDescriptor> descriptors) =>
        descriptors.Any(d => d.ServiceType == typeof(TService));

    private static bool HasOpenGenericBehavior(List<ServiceDescriptor> descriptors, Type openGenericBehavior) =>
        descriptors.Any(d => d.ImplementationType == openGenericBehavior);
}
