using Syntra.Execution;
using Syntra.Internal;
using Syntra.Notifications;

namespace Syntra.Caching;

/// <summary>
/// Static generic caches keyed by request, stream query, or notification type.
/// First access pays <see cref="Type.MakeGenericType"/> / <see cref="Activator.CreateInstance"/>; thereafter lock-free lookups.
/// </summary>
internal static class DelegateCache<TResponse>
{
    private static readonly ConcurrentDictionary<Type, IRequestHandlerWrapper<TResponse>> RequestExecutors = new();

    /// <summary>
    /// Returns the cached pipeline executor for <paramref name="requestType"/>.
    /// </summary>
    internal static IRequestHandlerWrapper<TResponse> GetOrCreateRequestPipeline(Type requestType)
        => RequestExecutors.GetOrAdd(requestType, static reqType =>
            (IRequestHandlerWrapper<TResponse>)Activator.CreateInstance(
                typeof(RequestPipelineExecutor<,>).MakeGenericType(reqType, typeof(TResponse)))!);
}

/// <summary>
/// Per-<typeparamref name="TResponse"/> cache for stream query executors.
/// </summary>
internal static class StreamDelegateCache<TResponse>
{
    private static readonly ConcurrentDictionary<Type, IStreamHandlerWrapper<TResponse>> StreamExecutors = new();

    internal static IStreamHandlerWrapper<TResponse> GetOrCreate(Type queryType)
        => StreamExecutors.GetOrAdd(queryType, static qType =>
            (IStreamHandlerWrapper<TResponse>)Activator.CreateInstance(
                typeof(StreamPipelineExecutor<,>).MakeGenericType(qType, typeof(TResponse)))!);
}

/// <summary>
/// Cache for notification dispatch bridges keyed by notification type.
/// </summary>
internal static class NotificationDelegateCache
{
    private static readonly ConcurrentDictionary<Type, INotificationHandlerWrapper> Cache = new();

    internal static INotificationHandlerWrapper GetOrCreate(Type notificationType)
        => Cache.GetOrAdd(notificationType, static nType =>
            (INotificationHandlerWrapper)Activator.CreateInstance(
                typeof(NotificationPipelineExecutor<>).MakeGenericType(nType))!);
}
