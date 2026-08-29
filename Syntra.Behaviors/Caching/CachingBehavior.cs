using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Syntra.Abstractions.Queries;
using Syntra.Behaviors.Serialization;

namespace Syntra.Behaviors.Caching;

/// <summary>
/// Response caching for requests implementing <see cref="ICacheableQuery"/>.
/// Only successful <see cref="Result"/> responses are written to the cache.
/// </summary>
public sealed class CachingBehavior<TRequest, TResponse>(
    IDistributedCache cache,
    ICacheKeyGenerator cacheKeyGenerator,
    CacheOptions cacheOptions,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly JsonSerializerOptions _jsonOptions = cacheOptions.SerializerOptions is null
        ? SyntraJsonSerializerOptions.CreateDefault()
        : SyntraJsonSerializerOptions.MergeWithResultConverters(cacheOptions.SerializerOptions);

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (request is not ICacheableQuery cacheableQuery)
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }

        var cacheKey = cacheKeyGenerator.BuildCacheKey(request, typeof(TRequest));

        var cachedBytes = await cache.GetAsync(cacheKey, cancellationToken).ConfigureAwait(false);

        if (cachedBytes is not null)
        {
            logger.LogDebug("Cache hit for key {CacheKey}", cacheKey);

            try
            {
                var cached = JsonSerializer.Deserialize<TResponse>(cachedBytes, _jsonOptions);
                if (cached is not null)
                {
                    return cached;
                }
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Failed to deserialize cached value for key {CacheKey}", cacheKey);
            }
        }

        logger.LogDebug("Cache miss for key {CacheKey}", cacheKey);

        var response = await next(cancellationToken).ConfigureAwait(false);

        if (response is Result result && result.IsFailure)
        {
            return response;
        }

        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(response, _jsonOptions);

            var entryOptions = new DistributedCacheEntryOptions();
            var ttl = cacheableQuery.CacheDuration ?? cacheOptions.DefaultExpiration;
            if (ttl.HasValue)
            {
                entryOptions.AbsoluteExpirationRelativeToNow = ttl;
            }

            await cache.SetAsync(cacheKey, bytes, entryOptions, cancellationToken).ConfigureAwait(false);

            logger.LogDebug(
                "Cached response for key {CacheKey}, expires in {Duration}",
                cacheKey,
                ttl?.ToString() ?? "default");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to cache response for key {CacheKey}", cacheKey);
        }

        return response;
    }
}
