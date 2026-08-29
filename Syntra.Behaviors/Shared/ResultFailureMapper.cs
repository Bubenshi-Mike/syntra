namespace Syntra.Behaviors.Shared;

/// <summary>
/// Builds failed <see cref="Result"/> / <see cref="Result{T}"/> instances for a closed
/// <typeparamref name="TResponse"/> type once, then reuses the compiled delegate — avoids
/// repeating reflection on every validation, authorization, or exception short-circuit.
/// </summary>
public static class ResultFailureMapper
{
    private static readonly ConcurrentDictionary<Type, Func<Error, object>> SingleErrorFactories = new();
    private static readonly ConcurrentDictionary<Type, Func<Error[], object>> MultiErrorFactories = new();

    /// <summary>
    /// Creates a failed result of <paramref name="responseType"/> carrying <paramref name="error"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="responseType"/> is not <see cref="Result"/> or <see cref="Result{T}"/>.
    /// </exception>
    public static object CreateFailureResult(Type responseType, Error error) =>
        SingleErrorFactories.GetOrAdd(responseType, BuildSingleErrorFactory)(error);

    /// <summary>
    /// Creates a failed result of <paramref name="responseType"/> carrying multiple <paramref name="errors"/>.
    /// </summary>
    public static object CreateFailureResult(Type responseType, IReadOnlyList<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        if (errors.Count == 0)
        {
            throw new ArgumentException("At least one error is required.", nameof(errors));
        }

        var array = errors as Error[] ?? errors.ToArray();
        return MultiErrorFactories.GetOrAdd(responseType, BuildMultiErrorFactory)(array);
    }

    /// <inheritdoc cref="CreateFailureResult(Type, Error)"/>
    public static TResponse CreateFailureResult<TResponse>(Error error) =>
        (TResponse)CreateFailureResult(typeof(TResponse), error);

    /// <inheritdoc cref="CreateFailureResult(Type, IReadOnlyList{Error})"/>
    public static TResponse CreateFailureResult<TResponse>(IReadOnlyList<Error> errors) =>
        (TResponse)CreateFailureResult(typeof(TResponse), errors);

    private static Func<Error, object> BuildSingleErrorFactory(Type responseType)
    {
        if (responseType == typeof(Result))
        {
            return static e => Result.Failure(e);
        }

        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var open = typeof(Result).GetMethods()
                .First(static m =>
                    m.Name == nameof(Result.Failure)
                    && m.IsGenericMethodDefinition
                    && m.GetParameters() is [{ ParameterType: var p }]
                    && p == typeof(Error));
            var closed = open.MakeGenericMethod(valueType);
            return e => closed.Invoke(null, [e])!;
        }

        throw new InvalidOperationException(
            $"Response type must be {nameof(Result)} or Result<T>. Actual: {responseType.FullName}");
    }

    private static Func<Error[], object> BuildMultiErrorFactory(Type responseType)
    {
        if (responseType == typeof(Result))
        {
            return static errors => Result.Failure(errors);
        }

        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var open = typeof(Result).GetMethods()
                .First(static m =>
                    m.Name == nameof(Result.Failure)
                    && m.IsGenericMethodDefinition
                    && m.GetParameters() is [{ ParameterType: var p }]
                    && p == typeof(Error[]));
            var closed = open.MakeGenericMethod(valueType);
            return errors => closed.Invoke(null, [errors])!;
        }

        throw new InvalidOperationException(
            $"Response type must be {nameof(Result)} or Result<T>. Actual: {responseType.FullName}");
    }
}
