using System.Text.Json;

namespace Syntra.Behaviors.Caching;

/// <summary>
/// Defaults for <see cref="CachingBehavior{TRequest,TResponse}"/>.
/// </summary>
public sealed class CacheOptions
{
    /// <summary>
    /// JSON serializer options for cache serialization. When <c>null</c>, <see cref="JsonSerializerDefaults.Web"/> is used.
    /// </summary>
    public JsonSerializerOptions? SerializerOptions { get; set; }

    /// <summary>
    /// When an <see cref="Abstractions.Queries.ICacheableQuery"/> has <c>null</c> <see cref="Abstractions.Queries.ICacheableQuery.CacheDuration"/>,
    /// this value is used for the distributed cache entry.
    /// </summary>
    public TimeSpan? DefaultExpiration { get; set; }
}
