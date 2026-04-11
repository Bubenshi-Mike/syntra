namespace Syntra.Abstractions.Pipelines;

/// <summary>
/// Represents the next step in a request processing pipeline.
/// </summary>
/// <typeparam name="TResponse">The response type flowing through the pipeline.</typeparam>
/// <remarks>
/// <para>
/// Each <c>IPipelineBehavior&lt;TRequest, TResponse&gt;</c> receives a
/// <see cref="RequestHandlerDelegate{TResponse}"/> that it must invoke to continue
/// the pipeline chain. The final delegate in the chain calls the actual handler.
/// </para>
/// <para>
/// Syntra.Behaviors that need to short-circuit (e.g. cache hits, authorization failures)
/// must simply not invoke <c>next</c> and return their own response instead.
/// </para>
/// </remarks>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(
    CancellationToken cancellationToken = default);
