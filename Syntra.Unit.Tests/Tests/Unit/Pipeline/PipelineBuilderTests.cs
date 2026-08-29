using Syntra.Abstractions.Pipelines;
using Syntra.Abstractions.Results;
using Syntra.Pipelines;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Pipeline;

public sealed class PipelineBuilderTests
{
    [Fact]
    public async Task Build_First_registered_behavior_runs_first_then_inner()
    {
        var order = new List<string>();
        var outer = new TagBehavior<UnitPingQuery, Result<int>>("outer", order);
        var inner = new TagBehavior<UnitPingQuery, Result<int>>("inner", order);
        var handler = new UnitPingQueryHandler();
        var request = new UnitPingQuery();

        var pipeline = PipelineBuilder<UnitPingQuery, Result<int>>.Build(
            handler,
            request,
            new IPipelineBehavior<UnitPingQuery, Result<int>>[] { outer, inner });

        var response = await pipeline(CancellationToken.None);

        Assert.True(response.IsSuccess);
        Assert.Equal(42, response.Value);
        Assert.Equal(new[] { "outer", "inner" }, order);
    }
}
