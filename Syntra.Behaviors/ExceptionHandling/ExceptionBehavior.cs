using Syntra.Behaviors.Shared;

namespace Syntra.Behaviors.ExceptionHandling;

/// <summary>
/// Catches unhandled exceptions from the inner pipeline, maps them to a failed
/// <see cref="Result"/> / <see cref="Result{T}"/>, and returns that value instead of throwing.
/// </summary>
/// <remarks>
/// <see cref="OperationCanceledException"/> is always re-thrown (cooperative cancellation).
/// Non-<see cref="Result"/>-based <typeparamref name="TResponse"/> types cause the exception to propagate.
/// </remarks>
public sealed class ExceptionBehavior<TRequest, TResponse>(
    ILogger<ExceptionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled exception in handler for {RequestName}. Converting to failure result.",
                typeof(TRequest).Name);

            var error = Error.FromException(ex);
            try
            {
                return (TResponse)ResultFailureMapper.CreateFailureResult(typeof(TResponse), error);
            }
            catch (InvalidOperationException)
            {
                throw ex;
            }
        }
    }
}
