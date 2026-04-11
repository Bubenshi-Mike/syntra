using Syntra.Abstractions.Requests;

namespace Syntra.Abstractions.Pipelines;

/// <summary>
/// An interceptor in the request-processing pipeline.
/// Syntra.Behaviors are composed in a chain; each must call <paramref name="next"/> to
/// continue, or return early to short-circuit.
/// </summary>
/// <typeparam name="TRequest">
/// The request type flowing through this behavior.
/// </typeparam>
/// <typeparam name="TResponse">
/// The response type produced at the end of the pipeline.
/// </typeparam>
/// <remarks>
/// <para>
/// Register behaviors in <c>Syntra.Syntra.DependencyInjection</c> via
/// <c>AddSyntra(...).AddBehavior&lt;TBehavior&gt;()</c>.
/// Syntra.Behaviors are applied in registration order: the first registered wraps the outermost layer.
/// </para>
/// <para>
/// Example cross-cutting concerns suited to behaviors:
/// <list type="bullet">
///   <item>Logging and telemetry (outermost — wraps everything)</item>
///   <item>Validation</item>
///   <item>Authorization</item>
///   <item>Caching (for queries)</item>
///   <item>Retry / circuit-breaker (via Polly)</item>
///   <item>Transaction management (innermost — wraps only the handler)</item>
/// </list>
/// </para>
/// </remarks>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request and optionally calls the next step in the pipeline.
    /// </summary>
    /// <param name="request">The incoming request. Never <c>null</c>.</param>
    /// <param name="next">
    /// Delegate that represents the remainder of the pipeline.
    /// Must be awaited to continue processing; do not invoke it when short-circuiting.
    /// </param>
    /// <param name="cancellationToken">Token to observe for cooperative cancellation.</param>
    /// <returns>
    /// A task resolving to <typeparamref name="TResponse"/>.
    /// Either the result of calling <paramref name="next"/>, or a locally-produced response.
    /// </returns>
    Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default);
}
