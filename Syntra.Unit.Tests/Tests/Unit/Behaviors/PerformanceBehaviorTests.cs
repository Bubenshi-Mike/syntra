using Microsoft.Extensions.Logging.Abstractions;
using Syntra.Abstractions.Results;
using Syntra.Behaviors.Performance;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class PerformanceBehaviorTests
{
    [Fact]
    public async Task Records_elapsed_and_returns_inner_result()
    {
        var options = new PerformanceBehaviorOptions { ThresholdMilliseconds = 10_000 };
        var behavior = new PerformanceBehavior<UnitPingQuery, Result<int>>(
            NullLogger<PerformanceBehavior<UnitPingQuery, Result<int>>>.Instance,
            options);

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(5)),
            CancellationToken.None).ConfigureAwait(false);

        Assert.True(response.IsSuccess);
        Assert.Equal(5, response.Value);
    }
}
