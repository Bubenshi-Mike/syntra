using Syntra.Exceptions;

namespace Syntra.Pipelines;

/// <summary>
/// Resolves the handler and pipeline behaviors from DI and composes them via <see cref="PipelineBuilder{TRequest,TResponse}"/>.
/// </summary>
internal static class PipelineComposer<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Resolves services, composes the pipeline, and starts execution.
    /// </summary>
    public static Task<TResponse> ComposeAndExecuteAsync(
        TRequest request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var handler = serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>()
            ?? throw new HandlerNotFoundException(typeof(TRequest));

        var behaviors = serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .ToArray();

        var pipeline = PipelineBuilder<TRequest, TResponse>.Build(handler, request, behaviors);
        return pipeline(cancellationToken);
    }
}
