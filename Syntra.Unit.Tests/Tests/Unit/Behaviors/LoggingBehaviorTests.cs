using Microsoft.Extensions.Logging.Abstractions;
using Syntra.Abstractions.Results;
using Syntra.Behaviors.Logging;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class LoggingBehaviorTests
{
    [Fact]
    public async Task Invokes_next_and_returns_response()
    {
        var behavior = new LoggingBehavior<UnitPingQuery, Result<int>>(
            NullLogger<LoggingBehavior<UnitPingQuery, Result<int>>>.Instance);

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(1)),
            CancellationToken.None).ConfigureAwait(false);

        Assert.True(response.IsSuccess);
    }
}
