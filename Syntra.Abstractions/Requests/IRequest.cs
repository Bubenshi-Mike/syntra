namespace Syntra.Abstractions.Requests;

/// <summary>
/// Non-generic marker for all Syntra requests.
/// Used in pipeline behavior constraints and mediator internals where
/// the response type is not statically known.
/// </summary>
/// <remarks>
/// Prefer the strongly-typed <see cref="IRequest{TResponse}"/> in handler declarations.
/// </remarks>
public interface IRequest { }

/// <summary>
/// Marker interface for a request that produces a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">
/// The type returned by the mediator when this request is dispatched.
/// For commands: <c>Result</c> or <c>Result&lt;T&gt;</c>.
/// For queries: <c>Result&lt;T&gt;</c>.
/// </typeparam>
public interface IRequest<out TResponse> : IRequest { }
