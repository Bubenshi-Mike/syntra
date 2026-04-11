using System.Text.Json;
using System.Linq;

namespace Syntra.Behaviors.Serialization;

/// <summary>Shared JSON options for behaviors that persist <c>Result</c> payloads.</summary>
public static class SyntraJsonSerializerOptions
{
    /// <summary>
    /// Web defaults plus <see cref="ResultJsonConverterFactory"/> for idempotency / cache round-trips.
    /// </summary>
    public static JsonSerializerOptions CreateDefault()
    {
        var o = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        o.Converters.Add(new ResultJsonConverterFactory());
        return o;
    }

    /// <summary>Merges <see cref="ResultJsonConverterFactory"/> into user-provided options (cloned).</summary>
    public static JsonSerializerOptions MergeWithResultConverters(JsonSerializerOptions user)
    {
        var o = new JsonSerializerOptions(user);
        if (!o.Converters.Any(static c => c is ResultJsonConverterFactory))
            o.Converters.Add(new ResultJsonConverterFactory());
        return o;
    }
}
