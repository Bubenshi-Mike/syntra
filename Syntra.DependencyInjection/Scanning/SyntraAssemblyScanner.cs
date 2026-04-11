using System.Reflection;
using FluentValidation;
using Scrutor;
using Syntra.Abstractions.Guards;
using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Notifications;
using Syntra.Behaviors.Audit;
using Syntra.Behaviors.Transactions;

namespace Syntra.DependencyInjection.Scanning;

/// <summary>
/// Scrutor-based assembly scanning for handlers, validators, and infrastructure ports.
/// </summary>
public static class SyntraAssemblyScanner
{
    /// <summary>
    /// Registers discovered types with <see cref="ServiceLifetime.Scoped"/> for handlers and
    /// <see cref="ServiceLifetime.Singleton"/> for writers / transaction managers unless overridden.
    /// </summary>
    public static void RegisterHandlersAndSupportServices(
        IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime handlerLifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(services);

        var assemblyArray = assemblies as Assembly[] ?? assemblies.ToArray();
        if (assemblyArray.Length == 0)
            return;

        // IRequestHandler<,> — commands & queries (includes ICommandHandler, IQueryHandler)
        services.Scan(scan => scan
            .FromAssemblies(assemblyArray)
            .AddClasses(classes => classes
                .AssignableTo(typeof(IRequestHandler<,>))
                .Where(HandlerConventions.IsRequestHandler))
            .AsImplementedInterfaces()
            .WithLifetime(handlerLifetime));

        // Stream handlers
        services.Scan(scan => scan
            .FromAssemblies(assemblyArray)
            .AddClasses(classes => classes
                .AssignableTo(typeof(IStreamQueryHandler<,>))
                .Where(HandlerConventions.IsStreamHandler))
            .AsImplementedInterfaces()
            .WithLifetime(handlerLifetime));

        // Notification handlers — register all implementations per notification type
        services.Scan(scan => scan
            .FromAssemblies(assemblyArray)
            .AddClasses(classes => classes
                .AssignableTo(typeof(INotificationHandler<>))
                .Where(HandlerConventions.IsNotificationHandler))
            .AsImplementedInterfaces()
            .WithLifetime(handlerLifetime));

        // FluentValidation validators
        services.Scan(scan => scan
            .FromAssemblies(assemblyArray)
            .AddClasses(classes => classes
                .AssignableTo(typeof(IValidator<>))
                .Where(HandlerConventions.IsValidator))
            .AsImplementedInterfaces()
            .WithLifetime(handlerLifetime));

        // Domain authorization handlers (IAuthorizedRequest guards)
        services.Scan(scan => scan
            .FromAssemblies(assemblyArray)
            .AddClasses(classes => classes
                .AssignableTo(typeof(IAuthorizationHandler<>))
                .Where(HandlerConventions.IsAuthorizationHandler))
            .AsImplementedInterfaces()
            .WithLifetime(handlerLifetime));

        // Optional infrastructure ports (register first concrete if multiple — app should register explicitly)
        services.Scan(scan => scan
            .FromAssemblies(assemblyArray)
            .AddClasses(classes => classes.AssignableTo<IAuditWriter>().Where(HandlerConventions.IsConcreteHandler))
            .AsImplementedInterfaces()
            .WithLifetime(ServiceLifetime.Singleton));

        services.Scan(scan => scan
            .FromAssemblies(assemblyArray)
            .AddClasses(classes => classes.AssignableTo<ITransactionManager>().Where(HandlerConventions.IsConcreteHandler))
            .AsImplementedInterfaces()
            .WithLifetime(ServiceLifetime.Singleton));
    }
}
