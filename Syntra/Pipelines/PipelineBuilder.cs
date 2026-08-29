using Syntra.Execution;

namespace Syntra.Pipelines;

/// <summary>
/// Pure pipeline composition: right-folds behaviors around the terminal handler delegate
/// so the first registered behavior becomes the outermost wrapper.
/// </summary>
internal static class PipelineBuilder<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Builds a <see cref="RequestHandlerDelegate{TResponse}"/> by chaining <paramref name="behaviors"/>
    /// from innermost (last registered) to outermost (first registered).
    /// </summary>
    public static RequestHandlerDelegate<TResponse> Build(
        IRequestHandler<TRequest, TResponse> handler,
        TRequest request,
        IPipelineBehavior<TRequest, TResponse>[] behaviors)
    {
        RequestHandlerDelegate<TResponse> terminal = ct =>
            HandlerInvoker<TRequest, TResponse>.Invoke(handler, request, ct);

        if (behaviors.Length == 0)
        {
            return terminal;
        }

        RequestHandlerDelegate<TResponse> pipeline = terminal;
        for (var i = behaviors.Length - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var next = pipeline;
            pipeline = ct => behavior.HandleAsync(request, next, ct);
        }

        return pipeline;
    }
}
