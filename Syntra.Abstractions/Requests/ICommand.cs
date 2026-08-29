namespace Syntra.Abstractions.Requests;

/// <summary>
/// Marker for a write-side command that has no return value beyond success/failure.
/// Handlers return <see cref="Result"/> (i.e. <c>Result&lt;Unit&gt;</c> semantics).
/// </summary>
/// <remarks>
/// Commands represent intent to change state. They must not be used for
/// read-only operations; use <see cref="IQuery{TResponse}"/> for those.
/// </remarks>
public interface ICommand : IRequest<Result> { }

/// <summary>
/// Marker for a write-side command that returns a value on success.
/// Handlers return <see cref="Result{TResponse}"/>.
/// </summary>
/// <typeparam name="TResponse">
/// The value produced by the command on success (e.g. a created entity ID).
/// </typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
