using Syntra.Abstractions.Pipelines;
using Syntra.Abstractions.Requests;

namespace Syntra.Unit.Tests.Tests.Unit.Pipeline;

internal sealed class TagBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly string _tag;
    private readonly List<string> _order;

    public TagBehavior(string tag, List<string> order)
    {
        _tag = tag;
        _order = order;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        _order.Add(_tag);
        return await next(cancellationToken).ConfigureAwait(false);
    }
}
