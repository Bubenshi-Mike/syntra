using Syntra.Abstractions.Results;
using Syntra.Diagnostics.Metrics;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class RequestMetricsBehaviorTests
{
    [Fact]
    public async Task Returns_inner_result_unchanged_on_success()
    {
        var behavior = new RequestMetricsBehavior<UnitPingQuery, Result<int>>();

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(5)),
            CancellationToken.None).ConfigureAwait(false);

        Assert.True(response.IsSuccess);
        Assert.Equal(5, response.Value);
    }

    [Fact]
    public async Task Rethrows_when_inner_pipeline_throws()
    {
        var behavior = new RequestMetricsBehavior<UnitPingQuery, Result<int>>();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            behavior.HandleAsync(
                new UnitPingQuery(),
                _ => throw new InvalidOperationException("boom"),
                CancellationToken.None));
    }
}
