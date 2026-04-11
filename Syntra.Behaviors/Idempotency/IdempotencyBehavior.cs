using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Syntra.Behaviors.Serialization;

namespace Syntra.Behaviors.Idempotency;

/// <summary>
/// Deduplicates commands implementing <see cref="Abstractions.Commands.IIdempotentCommand"/> or
/// <see cref="Abstractions.Commands.IIdempotentCommand{TResponse}"/> using <see cref="IDistributedCache"/>.
/// </summary>
public sealed class IdempotencyBehavior<TRequest, TResponse>(
    IDistributedCache cache,
    IdempotencyOptions options,
    ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly JsonSerializerOptions JsonOptions = SyntraJsonSerializerOptions.CreateDefault();

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (!IdempotencyKeyResolver.TryGetKey(request, out var key))
            return await next(cancellationToken).ConfigureAwait(false);

        var cacheKey = $"{options.CacheKeyPrefix}{typeof(TRequest).FullName}:{key:N}";

        var existing = await cache.GetAsync(cacheKey, cancellationToken).ConfigureAwait(false);
        if (existing is not null)
        {
            logger.LogDebug("Idempotency cache hit for {CacheKey}", cacheKey);
            var cached = JsonSerializer.Deserialize<TResponse>(existing, JsonOptions);
            if (cached is not null)
                return cached;
        }

        var response = await next(cancellationToken).ConfigureAwait(false);

        if (response is Result { IsFailure: true })
            return response;

        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(response, JsonOptions);
            var opts = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = options.CacheDuration,
            };
            await cache.SetAsync(cacheKey, bytes, opts, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to store idempotency response for {CacheKey}", cacheKey);
        }

        return response;
    }
}
