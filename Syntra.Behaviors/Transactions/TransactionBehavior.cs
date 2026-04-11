namespace Syntra.Behaviors.Transactions;

/// <summary>
/// Wraps the inner pipeline in <see cref="ITransactionManager"/> begin/commit/rollback
/// for requests implementing <see cref="ITransactionalRequest"/>.
/// </summary>
public sealed class TransactionBehavior<TRequest, TResponse>(
    ITransactionManager transactionManager,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (request is not ITransactionalRequest)
            return await next(cancellationToken).ConfigureAwait(false);

        await transactionManager.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var response = await next(cancellationToken).ConfigureAwait(false);
            await transactionManager.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Rolling back transaction for {RequestType}", typeof(TRequest).Name);
            await transactionManager.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }
}
