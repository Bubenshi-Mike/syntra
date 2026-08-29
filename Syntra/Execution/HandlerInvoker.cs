namespace Syntra.Execution;

/// <summary>
/// Compiles a strongly-typed delegate that invokes <see cref="IRequestHandler{TRequest,TResponse}.HandleAsync"/>
/// exactly once per closed generic pair, avoiding repeated reflection on the dispatch hot path.
/// </summary>
internal static class HandlerInvoker<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly Func<IRequestHandler<TRequest, TResponse>, TRequest, CancellationToken, Task<TResponse>>
        InvokeCore = CreateInvoker();

    private static Func<IRequestHandler<TRequest, TResponse>, TRequest, CancellationToken, Task<TResponse>> CreateInvoker()
    {
        var handlerParam = Expression.Parameter(typeof(IRequestHandler<TRequest, TResponse>), "handler");
        var requestParam = Expression.Parameter(typeof(TRequest), "request");
        var ctParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
        var method = typeof(IRequestHandler<TRequest, TResponse>).GetMethod(
            nameof(IRequestHandler<TRequest, TResponse>.HandleAsync),
            [typeof(TRequest), typeof(CancellationToken)])!;
        var body = Expression.Call(handlerParam, method, requestParam, ctParam);
        var lambda = Expression.Lambda<Func<IRequestHandler<TRequest, TResponse>, TRequest, CancellationToken, Task<TResponse>>>(
            body,
            handlerParam,
            requestParam,
            ctParam);
        return lambda.Compile();
    }

    /// <summary>
    /// Invokes the compiled delegate (same cost profile as a direct interface call, without <see cref="Reflection.MethodInfo.Invoke"/>).
    /// </summary>
    public static Task<TResponse> Invoke(
        IRequestHandler<TRequest, TResponse> handler,
        TRequest request,
        CancellationToken cancellationToken)
        => InvokeCore(handler, request, cancellationToken);
}
