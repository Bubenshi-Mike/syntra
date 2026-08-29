namespace Syntra.Abstractions.Requests;

/// <summary>
/// Marker for a read-side query that returns a value on success.
/// Handlers return <see cref="Result{TResponse}"/>.
/// </summary>
/// <typeparam name="TResponse">The type of value returned by the query.</typeparam>
/// <remarks>
/// Queries must be side-effect-free. Any handler implementing this interface
/// must not mutate application state.
/// </remarks>
public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
