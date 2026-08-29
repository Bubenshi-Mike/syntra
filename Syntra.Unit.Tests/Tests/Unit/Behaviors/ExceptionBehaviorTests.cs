using Microsoft.Extensions.Logging.Abstractions;
using Syntra.Abstractions.Results;
using Syntra.Behaviors.ExceptionHandling;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class ExceptionBehaviorTests
{
    [Fact]
    public async Task Passes_through_successful_next()
    {
        var behavior = new ExceptionBehavior<UnitPingQuery, Result<int>>(
            NullLogger<ExceptionBehavior<UnitPingQuery, Result<int>>>.Instance);

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(7)),
            CancellationToken.None);

        Assert.True(response.IsSuccess);
        Assert.Equal(7, response.Value);
    }
}
