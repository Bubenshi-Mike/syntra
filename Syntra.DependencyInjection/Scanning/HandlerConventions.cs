using FluentValidation;
using Syntra.Abstractions.Guards;

namespace Syntra.DependencyInjection.Scanning;

/// <summary>
/// Naming and type filters for Scrutor-based handler discovery.
/// </summary>
public static class HandlerConventions
{
    /// <summary>Concrete, non-abstract handler types only.</summary>
    public static bool IsConcreteHandler(Type type) =>
        type is { IsAbstract: false, IsGenericTypeDefinition: false };

    /// <summary>Matches <see cref="IRequestHandler{TRequest,TResponse}"/> implementations (commands, queries).</summary>
    public static bool IsRequestHandler(Type type) =>
        IsConcreteHandler(type) && ImplementsOpenGeneric(type, typeof(IRequestHandler<,>));

    /// <summary>Matches <see cref="IStreamQueryHandler{TQuery,TResponse}"/>.</summary>
    public static bool IsStreamHandler(Type type) =>
        IsConcreteHandler(type) && ImplementsOpenGeneric(type, typeof(IStreamQueryHandler<,>));

    /// <summary>Matches <see cref="INotificationHandler{TNotification}"/>.</summary>
    public static bool IsNotificationHandler(Type type) =>
        IsConcreteHandler(type) && ImplementsOpenGeneric(type, typeof(INotificationHandler<>));

    /// <summary>Matches <see cref="IValidator{T}"/>.</summary>
    public static bool IsValidator(Type type) =>
        IsConcreteHandler(type) && type.GetInterfaces().Any(static i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));

    /// <summary>Matches <see cref="IAuthorizationHandler{TRequest}"/>.</summary>
    public static bool IsAuthorizationHandler(Type type) =>
        IsConcreteHandler(type) && ImplementsOpenGeneric(type, typeof(IAuthorizationHandler<>));

    private static bool ImplementsOpenGeneric(Type type, Type openGenericDefinition)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == openGenericDefinition)
        {
            return true;
        }

        foreach (var iface in type.GetInterfaces())
        {
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == openGenericDefinition)
            {
                return true;
            }
        }

        return false;
    }
}
