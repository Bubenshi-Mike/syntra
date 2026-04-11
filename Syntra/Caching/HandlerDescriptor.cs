namespace Syntra.Caching;

/// <summary>
/// Captures compile-time metadata about a request/response pair.
/// Used by the caching and execution layers to avoid repeated
/// <c>typeof()</c> lookups and <c>GetGenericArguments()</c> calls on hot paths.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
internal static class HandlerDescriptor<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>The CLR type of the request.</summary>
    public static readonly Type RequestType = typeof(TRequest);

    /// <summary>The CLR type of the response.</summary>
    public static readonly Type ResponseType = typeof(TResponse);

    /// <summary>
    /// <c>true</c> when <typeparamref name="TResponse"/> is exactly <see cref="Result"/>.
    /// </summary>
    public static readonly bool IsResultResponse = typeof(TResponse) == typeof(Result);

    /// <summary>
    /// <c>true</c> when <typeparamref name="TResponse"/> is <c>Result&lt;T&gt;</c> for some T.
    /// </summary>
    public static readonly bool IsGenericResultResponse =
        typeof(TResponse).IsGenericType &&
        typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>);

    /// <summary>
    /// The inner value type when <see cref="IsGenericResultResponse"/> is <c>true</c>;
    /// <c>null</c> otherwise.
    /// </summary>
    public static readonly Type? ResultValueType =
        IsGenericResultResponse
            ? typeof(TResponse).GetGenericArguments()[0]
            : null;
}
