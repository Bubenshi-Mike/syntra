using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Syntra.DependencyInjection.Registration;

/// <summary>
/// Fluent builder returned by <c>AddSyntra()</c> that allows incremental registration
/// of handlers, behaviors, and notification publishers.
/// </summary>
public sealed class SyntraBuilder(IServiceCollection services)
{
    /// <summary>The underlying service collection being configured.</summary>
    public IServiceCollection Services { get; } = services;

    // =========================================================================
    // Handlers — manual registration
    // =========================================================================

    /// <summary>
    /// Registers a single request handler.
    /// </summary>
    public SyntraBuilder AddHandler<TRequest, TResponse, THandler>(
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TRequest : IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        Services.Add(ServiceDescriptor.Describe(
            typeof(IRequestHandler<TRequest, TResponse>),
            typeof(THandler),
            lifetime));
        return this;
    }

    /// <summary>
    /// Registers a single stream query handler.
    /// </summary>
    public SyntraBuilder AddStreamHandler<TQuery, TResponse, THandler>(
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TQuery : IStreamQuery<TResponse>
        where THandler : class, IStreamQueryHandler<TQuery, TResponse>
    {
        Services.Add(ServiceDescriptor.Describe(
            typeof(IStreamQueryHandler<TQuery, TResponse>),
            typeof(THandler),
            lifetime));
        return this;
    }

    /// <summary>
    /// Registers a notification handler.
    /// </summary>
    public SyntraBuilder AddNotificationHandler<TNotification, THandler>(
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TNotification : INotification
        where THandler : class, INotificationHandler<TNotification>
    {
        Services.Add(ServiceDescriptor.Describe(
            typeof(INotificationHandler<TNotification>),
            typeof(THandler),
            lifetime));
        return this;
    }

    // =========================================================================
    // Assembly scanning — bulk registration via Scrutor conventions
    // =========================================================================

    /// <summary>
    /// Scans <paramref name="assemblies"/> and registers all concrete, non-abstract
    /// classes that implement <c>IRequestHandler&lt;TRequest, TResponse&gt;</c>,
    /// <c>IStreamQueryHandler&lt;TQuery, TResponse&gt;</c>, or
    /// <c>INotificationHandler&lt;TNotification&gt;</c>.
    /// </summary>
    public SyntraBuilder AddHandlersFromAssemblies(
        ServiceLifetime lifetime = ServiceLifetime.Transient,
        params Assembly[] assemblies)
    {
        Services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes
                .AssignableTo(typeof(IRequestHandler<,>))
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition))
            .AsImplementedInterfaces()
            .WithLifetime(lifetime));

        Services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes
                .AssignableTo(typeof(IStreamQueryHandler<,>))
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition))
            .AsImplementedInterfaces()
            .WithLifetime(lifetime));

        Services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes
                .AssignableTo(typeof(INotificationHandler<>))
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition))
            .AsImplementedInterfaces()
            .WithLifetime(lifetime));

        return this;
    }

    /// <summary>
    /// Convenience overload: scans a single assembly.
    /// </summary>
    public SyntraBuilder AddHandlersFromAssembly(
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        => AddHandlersFromAssemblies(lifetime, assembly);

    // =========================================================================
    // Pipeline behaviors
    // =========================================================================

    /// <summary>
    /// Registers an open-generic pipeline behavior.
    /// Syntra.Behaviors are invoked in registration order (first registered = outermost layer).
    /// </summary>
    /// <param name="behaviorType">
    /// The open generic behavior type, e.g. <c>typeof(LoggingBehavior&lt;,&gt;)</c>.
    /// </param>
    public SyntraBuilder AddBehavior(
        Type behaviorType,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        Services.Add(ServiceDescriptor.Describe(
            typeof(IPipelineBehavior<,>),
            behaviorType,
            lifetime));
        return this;
    }

    /// <summary>
    /// Registers a closed or open-generic pipeline behavior.
    /// </summary>
    public SyntraBuilder AddBehavior<TBehavior>(
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        => AddBehavior(typeof(TBehavior), lifetime);

    // =========================================================================
    // Notification publisher
    // =========================================================================

    /// <summary>
    /// Replaces the default <see cref="SequentialNotificationPublisher"/> with a custom
    /// <see cref="INotificationPublisher"/> implementation.
    /// </summary>
    public SyntraBuilder UseNotificationPublisher<TPublisher>(
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TPublisher : class, INotificationPublisher
    {
        Services.Replace(ServiceDescriptor.Describe(
            typeof(INotificationPublisher),
            typeof(TPublisher),
            lifetime));
        return this;
    }
}
