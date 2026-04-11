using System.Diagnostics.CodeAnalysis;

namespace Syntra.Abstractions.Results;

/// <summary>
/// Represents the outcome of an operation that can either succeed or fail.
/// Implements the Railway-Oriented Programming pattern for explicit,
/// composable error handling without exception-based control flow.
/// </summary>
/// <remarks>
/// <para>
/// Use <see cref="Result"/> for commands and void-like operations.
/// Use <see cref="Result{TValue}"/> for operations that return a value on success.
/// </para>
/// <para>
/// Construction is intentionally restricted to the static factory methods
/// (<see cref="Success()"/>, <see cref="Failure(Error)"/>, etc.) to keep
/// the invariants enforced at one place.
/// </para>
/// </remarks>
public class Result
{
    // -------------------------------------------------------------------------
    // State
    // -------------------------------------------------------------------------

    /// <summary>
    /// <c>true</c> when the operation completed without error; <c>false</c> otherwise.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    /// <summary><c>true</c> when the operation failed.</summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// The first error that caused the failure, or <c>null</c> on success.
    /// Guarded by <see cref="IsFailure"/>; always non-null when <see cref="IsFailure"/> is <c>true</c>.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// All errors if multiple failures occurred simultaneously (e.g. bulk validation).
    /// Contains exactly the single <see cref="Error"/> when only one failure exists;
    /// empty on success.
    /// </summary>
    public IReadOnlyList<Error> Errors { get; }

    // -------------------------------------------------------------------------
    // Constructor (protected — use factory methods)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Initialises the result. Enforces mutual exclusivity of success and errors.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a successful result is given a non-<see cref="Results.Error.None"/> error,
    /// or when a failed result has no error. These are programming errors, not domain errors.
    /// </exception>
    protected Result(bool isSuccess, Error? error, IReadOnlyList<Error>? errors = null)
    {
        if (isSuccess && error is not null && error != Results.Error.None)
            throw new InvalidOperationException("A successful result cannot carry an error.");

        if (!isSuccess && (error is null || error == Results.Error.None))
            throw new InvalidOperationException("A failed result must carry at least one error.");

        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? (error is not null
            ? (IReadOnlyList<Error>)[error]
            : []);
    }

    // =========================================================================
    // Factory — Success
    // =========================================================================

    /// <summary>Creates a successful <see cref="Result"/>.</summary>
    public static Result Success() => new(true, null);

    /// <summary>Creates a successful <see cref="Result{TValue}"/> wrapping <paramref name="value"/>.</summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value to wrap.</param>
    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, null);

    // =========================================================================
    // Factory — Failure
    // =========================================================================

    /// <summary>Creates a failed <see cref="Result"/> with the given error.</summary>
    /// <param name="error">The error describing the failure.</param>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Creates a failed <see cref="Result"/> capturing multiple errors.
    /// The first element becomes <see cref="Error"/>.
    /// </summary>
    /// <param name="errors">One or more errors. Must not be empty.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is empty.</exception>
    public static Result Failure(params Error[] errors)
    {
        if (errors is null || errors.Length == 0)
            throw new ArgumentException("At least one error is required.", nameof(errors));

        return new(false, errors[0], errors);
    }

    /// <summary>Creates a failed <see cref="Result{TValue}"/> with the given error.</summary>
    /// <typeparam name="TValue">The value type (unused in the failure case).</typeparam>
    /// <param name="error">The error describing the failure.</param>
    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);

    /// <summary>Creates a failed <see cref="Result{TValue}"/> capturing multiple errors.</summary>
    /// <typeparam name="TValue">The value type (unused in the failure case).</typeparam>
    /// <param name="errors">One or more errors. Must not be empty.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is empty.</exception>
    public static Result<TValue> Failure<TValue>(params Error[] errors)
    {
        if (errors is null || errors.Length == 0)
            throw new ArgumentException("At least one error is required.", nameof(errors));

        return new(default, false, errors[0], errors);
    }

    // =========================================================================
    // Factory — shorthand domain helpers
    // =========================================================================

    /// <summary>Creates a failed <see cref="Result"/> with a <see cref="ErrorType.Validation"/> error.</summary>
    public static Result ValidationFailure(string code, string message, string? fieldName = null) =>
        Failure(Error.Validation(code, message, fieldName));

    /// <summary>Creates a failed <see cref="Result{TValue}"/> with a <see cref="ErrorType.Validation"/> error.</summary>
    public static Result<TValue> ValidationFailure<TValue>(string code, string message, string? fieldName = null) =>
        Failure<TValue>(Error.Validation(code, message, fieldName));

    /// <summary>Creates a failed <see cref="Result"/> with a <see cref="ErrorType.NotFound"/> error.</summary>
    public static Result NotFound(string code, string message) =>
        Failure(Error.NotFound(code, message));

    /// <summary>Creates a failed <see cref="Result{TValue}"/> with a <see cref="ErrorType.NotFound"/> error.</summary>
    public static Result<TValue> NotFound<TValue>(string code, string message) =>
        Failure<TValue>(Error.NotFound(code, message));

    // =========================================================================
    // Factory — from exception
    // =========================================================================

    /// <summary>
    /// Creates a failed <see cref="Result"/> from an exception.
    /// Prefer explicit <see cref="Error"/> values in domain code;
    /// use this only at infrastructure boundaries.
    /// </summary>
    /// <param name="exception">The exception to capture.</param>
    /// <param name="includeStackTrace">
    /// When <c>true</c>, the stack trace is appended to the error message.
    /// </param>
    public static Result FromException(Exception exception, bool includeStackTrace = false) =>
        Failure(Error.FromException(exception, includeStackTrace));

    /// <inheritdoc cref="FromException(Exception, bool)"/>
    /// <typeparam name="TValue">The value type.</typeparam>
    public static Result<TValue> FromException<TValue>(Exception exception, bool includeStackTrace = false) =>
        Failure<TValue>(Error.FromException(exception, includeStackTrace));

    // =========================================================================
    // Implicit conversion
    // =========================================================================

    /// <summary>Implicitly converts an <see cref="Results.Error"/> to a failed <see cref="Result"/>.</summary>
    public static implicit operator Result(Error error) => Failure(error);

    // =========================================================================
    // Pattern matching
    // =========================================================================

    /// <summary>
    /// Executes <paramref name="onSuccess"/> or <paramref name="onFailure"/> depending on state,
    /// and returns the result.
    /// </summary>
    /// <typeparam name="T">The return type of both branches.</typeparam>
    /// <param name="onSuccess">Invoked when this result is successful.</param>
    /// <param name="onFailure">Invoked with the primary <see cref="Error"/> when this result failed.</param>
    public T Match<T>(Func<T> onSuccess, Func<Error, T> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);

    /// <summary>
    /// Executes <paramref name="onSuccess"/> or <paramref name="onFailure"/> as a side effect.
    /// </summary>
    /// <param name="onSuccess">Action to run on success.</param>
    /// <param name="onFailure">Action to run on failure.</param>
    public void Match(Action onSuccess, Action<Error> onFailure)
    {
        if (IsSuccess) onSuccess(); else onFailure(Error);
    }

    // =========================================================================
    // Tap — side effects without altering the result
    // =========================================================================

    /// <summary>
    /// Runs <paramref name="action"/> if this result is successful, then returns <c>this</c>.
    /// Useful for logging or audit without breaking the pipeline chain.
    /// </summary>
    public Result Tap(Action action)
    {
        if (IsSuccess) action();
        return this;
    }

    /// <summary>Runs <paramref name="action"/> if this result failed, then returns <c>this</c>.</summary>
    public Result TapError(Action<Error> action)
    {
        if (IsFailure) action(Error);
        return this;
    }

    // =========================================================================
    // Ensure — inline validation
    // =========================================================================

    /// <summary>
    /// If the result is already failed, returns it unchanged.
    /// Otherwise evaluates <paramref name="predicate"/>; returns <paramref name="error"/>
    /// as a failure when the predicate is <c>false</c>.
    /// </summary>
    /// <param name="predicate">Condition that must hold for the result to remain successful.</param>
    /// <param name="error">Error to return when the condition is not met.</param>
    public Result Ensure(Func<bool> predicate, Error error)
    {
        if (IsFailure) return this;
        return predicate() ? this : Failure(error);
    }

    // =========================================================================
    // Combine — aggregate multiple results
    // =========================================================================

    /// <summary>
    /// Aggregates an array of results: succeeds only when all succeed;
    /// otherwise returns a single failed result containing all errors.
    /// </summary>
    /// <param name="results">Results to combine.</param>
    public static Result Combine(params Result[] results)
    {
        var errors = results
            .Where(static r => r.IsFailure)
            .SelectMany(static r => r.Errors)
            .ToArray();

        return errors.Length > 0 ? Failure(errors) : Success();
    }

    /// <inheritdoc cref="Combine(Result[])"/>
    public static Result Combine(IEnumerable<Result> results) =>
        Combine(results.ToArray());

    /// <summary>
    /// Combines the current result with <paramref name="result"/>.
    /// Returns the current result if it has already failed.
    /// </summary>
    public Result Combine(Result result) => IsFailure ? this : result;

    // =========================================================================
    // FirstFailureOrSuccess
    // =========================================================================

    /// <summary>
    /// Returns the first failed result in the sequence, or <see cref="Success()"/>
    /// when all results succeed. Short-circuits on the first failure.
    /// </summary>
    /// <param name="results">Results to evaluate in order.</param>
    public static Result FirstFailureOrSuccess(params Result[] results)
    {
        foreach (var result in results)
        {
            if (result.IsFailure) return result;
        }
        return Success();
    }
}

// =============================================================================
// Result<TValue>
// =============================================================================

/// <summary>
/// Represents the outcome of an operation that returns a value on success.
/// </summary>
/// <typeparam name="TValue">The type of the value returned when the operation succeeds.</typeparam>
/// <remarks>
/// Inherits all combinators from <see cref="Result"/> and adds value-aware overloads
/// (<see cref="Map{TNew}"/>, <see cref="Bind{TNew}"/>, <see cref="Tap(Action{TValue})"/>, etc.).
/// </remarks>
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Initialises the result with the given value and success/failure state.
    /// </summary>
    internal Result(TValue? value, bool isSuccess, Error? error, IReadOnlyList<Error>? errors = null)
        : base(isSuccess, error, errors)
    {
        _value = value;
    }

    // =========================================================================
    // Value access
    // =========================================================================

    /// <summary>
    /// Returns the encapsulated value.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the result is failed. Guard with <see cref="Result.IsSuccess"/> first,
    /// or use <see cref="GetValueOrDefault(TValue)"/> / pattern-match via <see cref="Match{T}(Func{TValue, T}, Func{Error, T})"/>.
    /// </exception>
    public TValue Value
    {
        get
        {
            if (IsFailure)
                throw new InvalidOperationException(
                    $"Cannot access {nameof(Value)} on a failed result. Error: {Error}");
            return _value!;
        }
    }

    /// <summary>
    /// Returns the value on success, or <paramref name="defaultValue"/> on failure.
    /// </summary>
    public TValue GetValueOrDefault(TValue defaultValue = default!) =>
        IsSuccess ? _value! : defaultValue;

    /// <summary>
    /// Returns the value on success, or the result of <paramref name="defaultValueFactory"/> on failure.
    /// The factory is not invoked on success.
    /// </summary>
    public TValue GetValueOrDefault(Func<TValue> defaultValueFactory) =>
        IsSuccess ? _value! : defaultValueFactory();

    /// <summary>
    /// Returns the value on success; throws the exception produced by
    /// <paramref name="exceptionFactory"/> on failure.
    /// </summary>
    /// <param name="exceptionFactory">Receives the failure error and must return an exception.</param>
    public TValue GetValueOrThrow(Func<Error, Exception> exceptionFactory)
    {
        if (IsFailure) throw exceptionFactory(Error);
        return _value!;
    }

    /// <summary>
    /// Tries to retrieve the value without throwing.
    /// </summary>
    /// <param name="value">Set to the value when the result is successful; otherwise <c>default</c>.</param>
    /// <returns><c>true</c> on success; <c>false</c> on failure.</returns>
    public bool TryGetValue([NotNullWhen(true)] out TValue? value)
    {
        value = IsSuccess ? _value : default;
        return IsSuccess;
    }

    // =========================================================================
    // Pattern matching
    // =========================================================================

    /// <summary>
    /// Executes <paramref name="onSuccess"/> with the value or <paramref name="onFailure"/>
    /// with the error, and returns the result.
    /// </summary>
    /// <typeparam name="T">Return type of both branches.</typeparam>
    public T Match<T>(Func<TValue, T> onSuccess, Func<Error, T> onFailure) =>
        IsSuccess ? onSuccess(_value!) : onFailure(Error);

    /// <summary>Executes one of two actions based on success or failure.</summary>
    public void Match(Action<TValue> onSuccess, Action<Error> onFailure)
    {
        if (IsSuccess) onSuccess(_value!); else onFailure(Error);
    }

    // =========================================================================
    // Map — transform the value (functor)
    // =========================================================================

    /// <summary>
    /// Projects the value to <typeparamref name="TNew"/> if successful.
    /// Propagates the failure unchanged when the result is failed.
    /// </summary>
    /// <typeparam name="TNew">The mapped value type.</typeparam>
    /// <param name="mapper">Pure transformation function.</param>
    public Result<TNew> Map<TNew>(Func<TValue, TNew> mapper)
    {
        if (IsFailure) return Failure<TNew>(Error);

        try { return Success(mapper(_value!)); }
        catch (Exception ex) { return FromException<TNew>(ex); }
    }

    /// <summary>
    /// Asynchronously projects the value to <typeparamref name="TNew"/> if successful.
    /// </summary>
    /// <typeparam name="TNew">The mapped value type.</typeparam>
    /// <param name="mapper">Async transformation function.</param>
    public async Task<Result<TNew>> MapAsync<TNew>(Func<TValue, Task<TNew>> mapper)
    {
        if (IsFailure) return Failure<TNew>(Error);

        try
        {
            var newValue = await mapper(_value!).ConfigureAwait(false);
            return Success(newValue);
        }
        catch (Exception ex) { return FromException<TNew>(ex); }
    }

    // =========================================================================
    // Bind — chain operations (monad)
    // =========================================================================

    /// <summary>
    /// Chains a function that itself returns a <see cref="Result{TNew}"/>.
    /// The binder is not called when this result is already failed.
    /// Enables Railway-Oriented Programming: each step either succeeds or short-circuits.
    /// </summary>
    /// <typeparam name="TNew">The next value type.</typeparam>
    /// <param name="binder">Function to call with the current value.</param>
    public Result<TNew> Bind<TNew>(Func<TValue, Result<TNew>> binder)
    {
        if (IsFailure) return Failure<TNew>(Error);

        try { return binder(_value!); }
        catch (Exception ex) { return FromException<TNew>(ex); }
    }

    /// <summary>
    /// Asynchronously chains a function that returns a <see cref="Result{TNew}"/>.
    /// </summary>
    /// <typeparam name="TNew">The next value type.</typeparam>
    /// <param name="binder">Async function to call with the current value.</param>
    public async Task<Result<TNew>> BindAsync<TNew>(Func<TValue, Task<Result<TNew>>> binder)
    {
        if (IsFailure) return Failure<TNew>(Error);

        try { return await binder(_value!).ConfigureAwait(false); }
        catch (Exception ex) { return FromException<TNew>(ex); }
    }

    /// <summary>
    /// Chains a function that returns a non-generic <see cref="Result"/>.
    /// Useful when the next step has no return value.
    /// </summary>
    /// <param name="binder">Function to call with the current value.</param>
    public Result Bind(Func<TValue, Result> binder)
    {
        if (IsFailure) return Failure(Error);

        try { return binder(_value!); }
        catch (Exception ex) { return FromException(ex); }
    }

    // =========================================================================
    // Tap — value-aware side effects
    // =========================================================================

    /// <summary>
    /// Runs <paramref name="action"/> with the value if successful, then returns <c>this</c>.
    /// </summary>
    public Result<TValue> Tap(Action<TValue> action)
    {
        if (IsSuccess) action(_value!);
        return this;
    }

    /// <summary>
    /// Asynchronously runs <paramref name="action"/> with the value if successful.
    /// </summary>
    public async Task<Result<TValue>> TapAsync(Func<TValue, Task> action)
    {
        if (IsSuccess) await action(_value!).ConfigureAwait(false);
        return this;
    }

    /// <summary>Runs <paramref name="action"/> with the error if failed, then returns <c>this</c>.</summary>
    public new Result<TValue> TapError(Action<Error> action)
    {
        if (IsFailure) action(Error);
        return this;
    }

    // =========================================================================
    // Ensure — value-aware inline validation
    // =========================================================================

    /// <summary>
    /// Validates the value with <paramref name="predicate"/>.
    /// Returns <paramref name="error"/> as a failure when the predicate returns <c>false</c>.
    /// Propagates existing failures without evaluating the predicate.
    /// </summary>
    public Result<TValue> Ensure(Func<TValue, bool> predicate, Error error)
    {
        if (IsFailure) return this;
        return predicate(_value!) ? this : Failure<TValue>(error);
    }

    /// <summary>Asynchronous variant of <see cref="Ensure(Func{TValue, bool}, Error)"/>.</summary>
    public async Task<Result<TValue>> EnsureAsync(Func<TValue, Task<bool>> predicate, Error error)
    {
        if (IsFailure) return this;
        return await predicate(_value!).ConfigureAwait(false) ? this : Failure<TValue>(error);
    }

    // =========================================================================
    // Error substitution
    // =========================================================================

    /// <summary>
    /// Replaces the failure error with <paramref name="newError"/>.
    /// Returns <c>this</c> unchanged on success.
    /// </summary>
    public Result<TValue> SwitchError(Error newError) =>
        IsSuccess ? this : Failure<TValue>(newError);

    /// <summary>
    /// Replaces the failure error by applying <paramref name="errorMapper"/>.
    /// Returns <c>this</c> unchanged on success.
    /// </summary>
    public Result<TValue> SwitchError(Func<Error, Error> errorMapper) =>
        IsSuccess ? this : Failure<TValue>(errorMapper(Error));

    // =========================================================================
    // Compensate — error recovery
    // =========================================================================

    /// <summary>
    /// Returns a successful result with <paramref name="fallbackValue"/> when this result failed.
    /// Returns <c>this</c> unchanged on success.
    /// </summary>
    public Result<TValue> Compensate(TValue fallbackValue) =>
        IsSuccess ? this : Success(fallbackValue);

    /// <summary>
    /// Computes a fallback value via <paramref name="fallbackFactory"/> when failed.
    /// Returns <c>this</c> unchanged on success.
    /// </summary>
    public Result<TValue> Compensate(Func<Error, TValue> fallbackFactory) =>
        IsSuccess ? this : Success(fallbackFactory(Error));

    /// <summary>
    /// Attempts recovery via <paramref name="recovery"/> when failed.
    /// Returns <c>this</c> unchanged on success.
    /// </summary>
    public Result<TValue> CompensateWith(Func<Error, Result<TValue>> recovery) =>
        IsSuccess ? this : recovery(Error);

    // =========================================================================
    // Implicit operators
    // =========================================================================

    /// <summary>Implicitly wraps a value in a successful <see cref="Result{TValue}"/>.</summary>
    public static implicit operator Result<TValue>(TValue value) => Success(value);

    /// <summary>Implicitly wraps an error in a failed <see cref="Result{TValue}"/>.</summary>
    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
}
