using Microsoft.Extensions.Logging.Abstractions;
using Syntra.Abstractions.Results;
using Syntra.Behaviors.Timeout;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class TimeoutBehaviorTests
{
    [Fact]
    public async Task Non_timeout_request_skips_linked_token()
    {
        var behavior = new TimeoutBehavior<UnitPingQuery, Result<int>>(
            new TimeoutOptions(),
            NullLogger<TimeoutBehavior<UnitPingQuery, Result<int>>>.Instance);

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(8)),
            CancellationToken.None).ConfigureAwait(false);

        Assert.True(response.IsSuccess);
    }
}
