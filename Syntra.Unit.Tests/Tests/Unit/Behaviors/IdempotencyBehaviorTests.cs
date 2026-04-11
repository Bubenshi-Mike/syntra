using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Syntra.Abstractions.Results;
using Syntra.Behaviors.Idempotency;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class IdempotencyBehaviorTests
{
    [Fact]
    public async Task Non_idempotent_request_skips_cache()
    {
        var cache = Substitute.For<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
        var behavior = new IdempotencyBehavior<UnitPingQuery, Result<int>>(
            cache,
            new IdempotencyOptions(),
            NullLogger<IdempotencyBehavior<UnitPingQuery, Result<int>>>.Instance);

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(4)),
            CancellationToken.None).ConfigureAwait(false);

        Assert.True(response.IsSuccess);
    }
}
