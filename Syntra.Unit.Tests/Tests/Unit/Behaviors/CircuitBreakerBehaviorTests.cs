using Microsoft.Extensions.Logging.Abstractions;
using Syntra.Abstractions.Results;
using Syntra.Behaviors.CircuitBreaker;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class CircuitBreakerBehaviorTests
{
    [Fact]
    public async Task Non_breakable_request_skips_breaker()
    {
        var behavior = new CircuitBreakerBehavior<UnitPingQuery, Result<int>>(
            new CircuitBreakerOptions(),
            NullLogger<CircuitBreakerBehavior<UnitPingQuery, Result<int>>>.Instance);

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(3)),
            CancellationToken.None).ConfigureAwait(false);

        Assert.True(response.IsSuccess);
    }
}
