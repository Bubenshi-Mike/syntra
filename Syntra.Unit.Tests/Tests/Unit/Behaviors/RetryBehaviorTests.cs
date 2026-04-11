using Microsoft.Extensions.Logging.Abstractions;
using Syntra.Abstractions.Results;
using Syntra.Behaviors.Retry;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class RetryBehaviorTests
{
    [Fact]
    public async Task Non_retryable_request_skips_policy()
    {
        var behavior = new RetryBehavior<UnitPingQuery, Result<int>>(
            new RetryOptions(),
            NullLogger<RetryBehavior<UnitPingQuery, Result<int>>>.Instance);

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(2)),
            CancellationToken.None).ConfigureAwait(false);

        Assert.True(response.IsSuccess);
    }
}
