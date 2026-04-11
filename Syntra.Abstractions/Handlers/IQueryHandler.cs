using Syntra.Abstractions.Requests;
using Syntra.Abstractions.Results;

namespace Syntra.Abstractions.Handlers;

/// <summary>
/// Handles a read-side query that returns a value on success.
/// </summary>
/// <typeparam name="TQuery">
/// The query type. Must implement <see cref="IQuery{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">The value returned by the query on success.</typeparam>
/// <remarks>
/// Query handlers must not produce side effects. If your implementation mutates
/// state, reconsider whether the operation is actually a command.
/// </remarks>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
    // Inherits: Task<Result<TResponse>> HandleAsync(TQuery request, CancellationToken cancellationToken)
}
