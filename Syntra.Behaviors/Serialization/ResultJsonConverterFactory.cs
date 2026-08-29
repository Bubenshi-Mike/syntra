using System.Text.Json;
using System.Text.Json.Serialization;

namespace Syntra.Behaviors.Serialization;

/// <summary>
/// Enables <see cref="Result"/> / <see cref="Result{TValue}"/> round-tripping for distributed cache and idempotency.
/// </summary>
public sealed class ResultJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert == typeof(Result)
        || typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Result<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(Result))
        {
            return new NonGenericResultJsonConverter();
        }

        var valueType = typeToConvert.GetGenericArguments()[0];
        return (JsonConverter)Activator.CreateInstance(typeof(GenericResultJsonConverter<>).MakeGenericType(valueType))!;
    }

    private sealed class NonGenericResultJsonConverter : JsonConverter<Result>
    {
        public override Result Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            var isSuccess = root.GetProperty("isSuccess").GetBoolean();
            if (isSuccess)
            {
                return Result.Success();
            }

            var error = root.GetProperty("error").Deserialize<Error>(options)
                        ?? Error.Failure("Result.Deserialize", "Missing error payload.");
            return Result.Failure(error);
        }

        public override void Write(Utf8JsonWriter writer, Result value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteBoolean("isSuccess", value.IsSuccess);
            if (value.IsFailure)
            {
                writer.WritePropertyName("error");
                JsonSerializer.Serialize(writer, value.Error, options);
            }

            writer.WriteEndObject();
        }
    }

    private sealed class GenericResultJsonConverter<TValue> : JsonConverter<Result<TValue>>
    {
        public override Result<TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            var isSuccess = root.GetProperty("isSuccess").GetBoolean();
            if (isSuccess)
            {
                if (!root.TryGetProperty("value", out var valueEl))
                {
                    return Result.Failure<TValue>(Error.Failure("Result.Deserialize", "Missing value for successful Result<T>."));
                }

                var v = valueEl.Deserialize<TValue>(options);
                return Result.Success(v!);
            }

            var error = root.GetProperty("error").Deserialize<Error>(options)
                        ?? Error.Failure("Result.Deserialize", "Missing error payload.");
            return Result.Failure<TValue>(error);
        }

        public override void Write(Utf8JsonWriter writer, Result<TValue> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteBoolean("isSuccess", value.IsSuccess);
            if (value.IsSuccess)
            {
                writer.WritePropertyName("value");
                JsonSerializer.Serialize(writer, value.Value, options);
            }
            else
            {
                writer.WritePropertyName("error");
                JsonSerializer.Serialize(writer, value.Error, options);
            }

            writer.WriteEndObject();
        }
    }
}
