using Syntra.Abstractions.Requests;
using Syntra.Internal;
using Syntra.Pipelines;

namespace Syntra.Execution;

/// <summary>
/// Cached per request type: resolves DI services and runs <see cref="PipelineComposer{TRequest,TResponse}"/>.
/// </summary>
/// <typeparam name="TRequest">The concrete request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
internal sealed class RequestPipelineExecutor<TRequest, TResponse> : IRequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public Task<TResponse> HandleAsync(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var typedRequest = (TRequest)request;
        return PipelineComposer<TRequest, TResponse>.ComposeAndExecuteAsync(
            typedRequest,
            serviceProvider,
            cancellationToken);
    }
}
