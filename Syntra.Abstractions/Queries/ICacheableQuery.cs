namespace Syntra.Abstractions.Queries;

/// <summary>
/// Non-generic marker for cacheable queries.
/// Used by <c>CachingBehavior</c> in a simple <c>is</c> check so that the behavior
/// does not need to know the query's inner value type — only that it is cacheable.
/// </summary>
/// <remarks>
/// Do not implement this interface directly. Implement <see cref="ICacheableQuery{TResponse}"/>
/// instead; it inherits this marker automatically.
/// </remarks>
public interface ICacheableQuery
{
    /// <summary>A unique, deterministic cache key for this query instance.</summary>
    string CacheKey { get; }

    /// <summary>
    /// The absolute duration for which the cached response is valid.
    /// <c>null</c> defers to the cache provider's default expiry policy.
    /// </summary>
    TimeSpan? CacheDuration { get; }

    /// <summary>
    /// Optional cache region / tag used for bulk invalidation.
    /// When <c>null</c>, no region is associated with the entry.
    /// </summary>
    string? CacheRegion => null;
}

/// <summary>
/// Opt-in contract for queries whose results may be cached.
/// </summary>
/// <typeparam name="TResponse">
/// The query value type (not the Result-wrapped type).
/// E.g. use <c>ICacheableQuery&lt;ProductDto&gt;</c> on a <c>IQuery&lt;ProductDto&gt;</c> query.
/// </typeparam>
/// <remarks>
/// <para>
/// This interface intentionally does NOT extend <c>IRequest&lt;TResponse&gt;</c> to avoid
/// a duplicate-interface-instantiation conflict with <c>IQuery&lt;TResponse&gt;</c>
/// (which already resolves to <c>IRequest&lt;Result&lt;TResponse&gt;&gt;</c>).
/// The <see cref="ICacheableQuery"/> non-generic marker is what the caching behavior
/// checks at runtime — the generic parameter here is purely for caller convenience.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public sealed record GetProductByIdQuery(Guid Id)
///     : IQuery&lt;ProductDto&gt;, ICacheableQuery&lt;ProductDto&gt;
/// {
///     public string CacheKey =&gt; $"product:{Id}";
///     public TimeSpan? CacheDuration =&gt; TimeSpan.FromMinutes(5);
/// }
/// </code>
/// </example>
public interface ICacheableQuery<TResponse> : ICacheableQuery
{
}
