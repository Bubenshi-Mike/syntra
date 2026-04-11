namespace Syntra.Internal;

/// <summary>
/// Non-generic marker interface for cached handler wrappers.
/// Used as the dictionary value type in <see cref="Caching.DelegateCache{TResponse}"/>.
/// </summary>
internal interface IRequestHandlerWrapper { }

/// <summary>
/// Typed wrapper that bridges the non-generic mediator dispatch path to the
/// strongly-typed <c>IRequestHandler&lt;TRequest, TResponse&gt;</c> and
/// <c>IPipelineBehavior&lt;TRequest, TResponse&gt;</c> resolved from DI.
/// </summary>
/// <typeparam name="TResponse">The response type produced by the pipeline.</typeparam>
internal interface IRequestHandlerWrapper<TResponse> : IRequestHandlerWrapper
{
    /// <summary>
    /// Executes the full pipeline (behaviors + handler) for the given request.
    /// </summary>
    /// <param name="request">The request, cast internally to the concrete request type.</param>
    /// <param name="serviceProvider">Scoped DI container for resolving handlers and behaviors.</param>
    /// <param name="cancellationToken">Token to observe for cooperative cancellation.</param>
    Task<TResponse> HandleAsync(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}
