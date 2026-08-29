namespace Syntra.Execution;

/// <summary>
/// Compiles a delegate that invokes <see cref="IStreamQueryHandler{TQuery,TResponse}.HandleAsync"/>
/// once per closed query/response pair.
/// </summary>
internal static class StreamInvoker<TQuery, TResponse>
    where TQuery : IStreamQuery<TResponse>
{
    private static readonly Func<IStreamQueryHandler<TQuery, TResponse>, TQuery, CancellationToken, IAsyncEnumerable<TResponse>>
        InvokeCore = CreateInvoker();

    private static Func<IStreamQueryHandler<TQuery, TResponse>, TQuery, CancellationToken, IAsyncEnumerable<TResponse>> CreateInvoker()
    {
        var handlerParam = Expression.Parameter(typeof(IStreamQueryHandler<TQuery, TResponse>), "handler");
        var queryParam = Expression.Parameter(typeof(TQuery), "query");
        var ctParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
        var method = typeof(IStreamQueryHandler<TQuery, TResponse>).GetMethod(
            nameof(IStreamQueryHandler<TQuery, TResponse>.HandleAsync),
            [typeof(TQuery), typeof(CancellationToken)])!;
        var body = Expression.Call(handlerParam, method, queryParam, ctParam);
        var lambda = Expression.Lambda<Func<IStreamQueryHandler<TQuery, TResponse>, TQuery, CancellationToken, IAsyncEnumerable<TResponse>>>(
            body,
            handlerParam,
            queryParam,
            ctParam);
        return lambda.Compile();
    }

    public static IAsyncEnumerable<TResponse> Invoke(
        IStreamQueryHandler<TQuery, TResponse> handler,
        TQuery query,
        CancellationToken cancellationToken)
        => InvokeCore(handler, query, cancellationToken);
}
