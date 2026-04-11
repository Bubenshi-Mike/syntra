using Syntra.Abstractions.Requests;

namespace Syntra.Abstractions.Handlers;

/// <summary>
/// Core handler abstraction. All concrete handlers ultimately implement this interface.
/// </summary>
/// <typeparam name="TRequest">
/// The request type. Must implement <see cref="IRequest{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The response type produced by this handler (e.g. <c>Result</c> or <c>Result&lt;T&gt;</c>).
/// </typeparam>
/// <remarks>
/// Prefer the domain-specific sub-interfaces (<see cref="ICommandHandler{TCommand}"/>,
/// <see cref="IQueryHandler{TQuery, TResponse}"/>, etc.) which communicate intent
/// through their type names and carry tighter generic constraints.
/// </remarks>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request and returns a response.
    /// </summary>
    /// <param name="request">The request to handle. Never <c>null</c>.</param>
    /// <param name="cancellationToken">Token to observe for cooperative cancellation.</param>
    /// <returns>A task that resolves to the handler's response.</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}
