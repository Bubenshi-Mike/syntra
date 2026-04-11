using Syntra.Abstractions.Pipelines;
using Syntra.Abstractions.Results;
using Syntra.Pipelines;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Pipeline;

/// <summary>Documents that registration order is outermost-first (see <see cref="PipelineBuilder{TRequest,TResponse}"/>).</summary>
public sealed class PipelineOrderTests
{
    [Fact]
    public async Task Outer_wraps_inner_wraps_handler()
    {
        var order = new List<string>();
        var behaviors = new IPipelineBehavior<UnitPingQuery, Result<int>>[]
        {
            new TagBehavior<UnitPingQuery, Result<int>>("a", order),
            new TagBehavior<UnitPingQuery, Result<int>>("b", order),
        };

        var pipeline = PipelineBuilder<UnitPingQuery, Result<int>>.Build(
            new UnitPingQueryHandler(),
            new UnitPingQuery(),
            behaviors);

        _ = await pipeline(CancellationToken.None).ConfigureAwait(false);

        Assert.Equal(new[] { "a", "b" }, order);
    }
}
