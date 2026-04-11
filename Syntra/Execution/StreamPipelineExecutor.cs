using Syntra.Abstractions.Requests;
using Syntra.Exceptions;
using Syntra.Internal;

namespace Syntra.Execution;

/// <summary>
/// Cached per stream query type: resolves <see cref="IStreamQueryHandler{TQuery,TResponse}"/> and
/// invokes via <see cref="StreamInvoker{TQuery,TResponse}"/>.
/// </summary>
internal sealed class StreamPipelineExecutor<TQuery, TResponse> : IStreamHandlerWrapper<TResponse>
    where TQuery : IStreamQuery<TResponse>
{
    /// <inheritdoc />
    public IAsyncEnumerable<TResponse> HandleAsync(
        object query,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var typedQuery = (TQuery)query;
        var handler = serviceProvider.GetService<IStreamQueryHandler<TQuery, TResponse>>()
            ?? throw new HandlerNotFoundException(typeof(TQuery));
        return StreamInvoker<TQuery, TResponse>.Invoke(handler, typedQuery, cancellationToken);
    }
}
