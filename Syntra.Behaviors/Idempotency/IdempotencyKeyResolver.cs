using System.Reflection;

namespace Syntra.Behaviors.Idempotency;

/// <summary>
/// Resolves idempotency keys from <see cref="IIdempotentCommand"/> / <see cref="IIdempotentCommand{TResponse}"/>.
/// </summary>
internal static class IdempotencyKeyResolver
{
    public static bool TryGetKey(object request, out Guid key)
    {
        if (request is IIdempotentCommand c)
        {
            key = c.IdempotencyKey;
            return true;
        }

        foreach (var itf in request.GetType().GetInterfaces())
        {
            if (!itf.IsGenericType)
            {
                continue;
            }

            if (itf.GetGenericTypeDefinition() != typeof(IIdempotentCommand<>))
            {
                continue;
            }

            var prop = itf.GetProperty(nameof(IIdempotentCommand.IdempotencyKey), BindingFlags.Public | BindingFlags.Instance);
            if (prop?.GetValue(request) is Guid g)
            {
                key = g;
                return true;
            }
        }

        key = default;
        return false;
    }
}
