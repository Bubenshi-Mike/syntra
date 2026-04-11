using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Syntra.Abstractions.Results;
using Syntra.Behaviors.Caching;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class CachingBehaviorTests
{
    [Fact]
    public async Task Non_cacheable_request_skips_store()
    {
        var cache = Substitute.For<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
        var keys = Substitute.For<ICacheKeyGenerator>();
        var options = Microsoft.Extensions.Options.Options.Create(new CacheOptions());
        var behavior = new CachingBehavior<UnitPingQuery, Result<int>>(
            cache,
            keys,
            options.Value,
            NullLogger<CachingBehavior<UnitPingQuery, Result<int>>>.Instance);

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(3)),
            CancellationToken.None).ConfigureAwait(false);

        Assert.True(response.IsSuccess);
        await cache.DidNotReceive()
            .GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
