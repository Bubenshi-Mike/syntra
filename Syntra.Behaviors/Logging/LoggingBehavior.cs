using System.Diagnostics;

namespace Syntra.Behaviors.Logging;

/// <summary>
/// Structured logging for request start, completion, elapsed time, and success/failure (for <see cref="Result"/>).
/// </summary>
/// <remarks>
/// Register as the outermost behavior so elapsed time covers the full pipeline.
/// </remarks>
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly string RequestName = typeof(TRequest).Name;

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling {RequestName} {@Request}", RequestName, request);

        var sw = Stopwatch.StartNew();
        try
        {
            var response = await next(cancellationToken).ConfigureAwait(false);
            sw.Stop();

            if (response is Result result)
            {
                if (result.IsSuccess)
                {
                    logger.LogInformation(
                        "Handled {RequestName} in {ElapsedMs}ms — Success",
                        RequestName, sw.ElapsedMilliseconds);
                }
                else
                {
                    logger.LogWarning(
                        "Handled {RequestName} in {ElapsedMs}ms — Failure: {ErrorCode} {ErrorMessage}",
                        RequestName, sw.ElapsedMilliseconds,
                        result.Error.Code, result.Error.Message);
                }
            }
            else
            {
                logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMs}ms",
                    RequestName, sw.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            sw.Stop();
            logger.LogError(
                ex,
                "Unhandled exception in {RequestName} after {ElapsedMs}ms",
                RequestName, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
