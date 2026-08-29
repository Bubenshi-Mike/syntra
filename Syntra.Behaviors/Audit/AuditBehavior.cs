namespace Syntra.Behaviors.Audit;

/// <summary>
/// After the inner pipeline completes, writes an <see cref="AuditEntry"/> for requests implementing
/// <see cref="IAuditableRequest"/> via <see cref="IAuditWriter"/>.
/// </summary>
public sealed class AuditBehavior<TRequest, TResponse>(
    IAuditWriter auditWriter,
    ILogger<AuditBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (request is not IAuditableRequest auditable)
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }

        var response = await next(cancellationToken).ConfigureAwait(false);

        string? errorCode = null;
        string? errorMessage = null;
        var succeeded = true;

        if (response is Result r)
        {
            succeeded = r.IsSuccess;
            if (r.IsFailure)
            {
                errorCode = r.Error.Code;
                errorMessage = r.Error.Message;
            }
        }

        var entry = new AuditEntry(
            typeof(TRequest).FullName ?? typeof(TRequest).Name,
            auditable.InitiatedBy,
            auditable.RequestedAt,
            auditable.Reason,
            succeeded,
            errorCode,
            errorMessage);

        try
        {
            await auditWriter.WriteAsync(entry, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Audit writer failed for {RequestType}", typeof(TRequest).Name);
        }

        return response;
    }
}
