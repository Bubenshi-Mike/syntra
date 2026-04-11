namespace Syntra.Abstractions.Results;

/// <summary>
/// Extension methods for <see cref="Result"/> and <see cref="Result{TValue}"/> that lift
/// synchronous operations into asynchronous (Task-based) chains.
/// These extensions keep pipeline code linear instead of pyramid-nested.
/// </summary>
public static class ResultExtensions
{
    // =========================================================================
    // Bind
    // =========================================================================

    /// <summary>
    /// Awaits <paramref name="resultTask"/>, then calls the async binder if successful.
    /// Short-circuits with the failure if the result is already failed.
    /// </summary>
    public static async Task<Result<TNew>> BindAsync<TValue, TNew>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<Result<TNew>>> binder)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.BindAsync(binder).ConfigureAwait(false);
    }

    /// <summary>
    /// Awaits <paramref name="resultTask"/>, then calls the synchronous binder if successful.
    /// </summary>
    public static async Task<Result<TNew>> Bind<TValue, TNew>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Result<TNew>> binder)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Bind(binder);
    }

    // =========================================================================
    // Map
    // =========================================================================

    /// <summary>
    /// Awaits <paramref name="resultTask"/>, then asynchronously projects the value if successful.
    /// </summary>
    public static async Task<Result<TNew>> MapAsync<TValue, TNew>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<TNew>> mapper)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.MapAsync(mapper).ConfigureAwait(false);
    }

    /// <summary>
    /// Awaits <paramref name="resultTask"/>, then synchronously projects the value if successful.
    /// </summary>
    public static async Task<Result<TNew>> Map<TValue, TNew>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, TNew> mapper)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Map(mapper);
    }

    // =========================================================================
    // Tap
    // =========================================================================

    /// <summary>
    /// Awaits <paramref name="resultTask"/>, then runs an async side-effect if successful.
    /// Returns the original result unchanged.
    /// </summary>
    public static async Task<Result<TValue>> TapAsync<TValue>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.TapAsync(action).ConfigureAwait(false);
    }

    /// <summary>
    /// Awaits <paramref name="resultTask"/>, then runs a synchronous side-effect if successful.
    /// Returns the original result unchanged.
    /// </summary>
    public static async Task<Result<TValue>> Tap<TValue>(
        this Task<Result<TValue>> resultTask,
        Action<TValue> action)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Tap(action);
    }

    // =========================================================================
    // Ensure
    // =========================================================================

    /// <summary>
    /// Awaits <paramref name="resultTask"/>, then validates the value with an async predicate.
    /// </summary>
    public static async Task<Result<TValue>> EnsureAsync<TValue>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<bool>> predicate,
        Error error)
    {
        var result = await resultTask.ConfigureAwait(false);
        return await result.EnsureAsync(predicate, error).ConfigureAwait(false);
    }

    /// <summary>
    /// Awaits <paramref name="resultTask"/>, then validates the value with a synchronous predicate.
    /// </summary>
    public static async Task<Result<TValue>> Ensure<TValue>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, bool> predicate,
        Error error)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Ensure(predicate, error);
    }

    // =========================================================================
    // Match
    // =========================================================================

    /// <summary>
    /// Awaits <paramref name="resultTask"/>, then pattern-matches on the value result.
    /// </summary>
    public static async Task<T> Match<TValue, T>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, T> onSuccess,
        Func<Error, T> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Match(onSuccess, onFailure);
    }

    /// <summary>
    /// Awaits <paramref name="resultTask"/>, then pattern-matches on the void result.
    /// </summary>
    public static async Task<T> Match<T>(
        this Task<Result> resultTask,
        Func<T> onSuccess,
        Func<Error, T> onFailure)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.Match(onSuccess, onFailure);
    }

    // =========================================================================
    // ToResult — conversion helpers
    // =========================================================================

    /// <summary>
    /// Wraps a <see cref="Task{T}"/> execution in a <see cref="Result{T}"/>,
    /// catching any exception as an <see cref="ErrorType.Unexpected"/> failure.
    /// Use at infrastructure entry points where exceptions can escape.
    /// </summary>
    /// <typeparam name="T">The task's value type.</typeparam>
    /// <param name="task">The task to wrap.</param>
    /// <param name="includeStackTrace">When <c>true</c>, stack trace is embedded in the error message.</param>
    public static async Task<Result<T>> ToResult<T>(
        this Task<T> task,
        bool includeStackTrace = false)
    {
        try
        {
            return Result.Success(await task.ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return Result.FromException<T>(ex, includeStackTrace);
        }
    }

    /// <summary>
    /// Converts a nullable reference to a <see cref="Result{T}"/>.
    /// Returns success when the value is non-null; otherwise the given <paramref name="error"/>.
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, Error error) where T : class =>
        value is not null ? Result.Success(value) : Result.Failure<T>(error);

    /// <summary>
    /// Converts a nullable value type to a <see cref="Result{T}"/>.
    /// Returns success when the value has a value; otherwise the given <paramref name="error"/>.
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, Error error) where T : struct =>
        value.HasValue ? Result.Success(value.Value) : Result.Failure<T>(error);

    // =========================================================================
    // Combine (async)
    // =========================================================================

    /// <summary>
    /// Awaits all tasks and combines their results.
    /// All must succeed; otherwise returns a single failure carrying all errors.
    /// </summary>
    public static async Task<Result> CombineAsync(this IEnumerable<Task<Result>> tasks)
    {
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);
        return Result.Combine(results);
    }

    /// <summary>
    /// Awaits all tasks, collecting values from all successful results.
    /// Returns a single failure carrying all errors if any task fails.
    /// </summary>
    public static async Task<Result<IReadOnlyList<T>>> CombineAsync<T>(
        this IEnumerable<Task<Result<T>>> tasks)
    {
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        var errors = results
            .Where(static r => r.IsFailure)
            .SelectMany(static r => r.Errors)
            .ToArray();

        if (errors.Length > 0)
            return Result.Failure<IReadOnlyList<T>>(errors);

        IReadOnlyList<T> values = results.Select(static r => r.Value).ToList();
        return Result.Success(values);
    }
}
