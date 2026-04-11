using System.Reflection;
using Syntra.DependencyInjection.Scanning;

namespace Syntra.DependencyInjection.Registration;

/// <summary>
/// Registers all CQRS handlers and supporting types from assemblies using <see cref="SyntraAssemblyScanner"/>.
/// </summary>
public static class HandlerRegistrar
{
    /// <summary>
    /// Scans <paramref name="assemblies"/> using <see cref="HandlerConventions"/> and Scrutor.
    /// </summary>
    public static void Register(IServiceCollection services, IEnumerable<Assembly> assemblies, ServiceLifetime handlerLifetime = ServiceLifetime.Scoped)
    {
        SyntraAssemblyScanner.RegisterHandlersAndSupportServices(services, assemblies, handlerLifetime);
    }
}
