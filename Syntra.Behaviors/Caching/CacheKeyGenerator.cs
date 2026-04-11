using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Syntra.Abstractions.Queries;

namespace Syntra.Behaviors.Caching;

/// <summary>
/// Produces distributed-cache keys for request instances.
/// </summary>
public interface ICacheKeyGenerator
{
    /// <summary>
    /// Builds a stable cache key for <paramref name="request"/> (same logical query → same key).
    /// </summary>
    string BuildCacheKey(object request, Type requestType);
}

/// <summary>
/// Uses <see cref="ICacheableQuery.CacheKey"/> when implemented; otherwise falls back to a type-qualified JSON payload hash.
/// </summary>
public sealed class DefaultCacheKeyGenerator(CacheOptions? options = null) : ICacheKeyGenerator
{
    private readonly JsonSerializerOptions _jsonOptions =
        options?.SerializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);

    /// <inheritdoc />
    public string BuildCacheKey(object request, Type requestType)
    {
        if (request is ICacheableQuery q)
            return q.CacheKey;

        var json = JsonSerializer.Serialize(request, requestType, _jsonOptions);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        var b64 = Convert.ToHexString(hash);
        return $"{requestType.FullName}:{b64}";
    }
}
