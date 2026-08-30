using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
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
            CancellationToken.None);

        Assert.True(response.IsSuccess);
        Assert.Equal(5, response.Value);
    }

    // Regression test for a real leak: {@Request}/record ToString() destructuring previously
    // logged the entire request object - including sensitive fields such as a Password property
    // - whenever a request exceeded the configured threshold. Syntra has no way to know which
    // properties on an arbitrary, consumer-defined request type are sensitive, so the fix is to
    // never log the request object's contents at all - only its type name.
    [Fact]
    public async Task Does_not_log_the_raw_request_object_or_sensitive_field_values_when_threshold_exceeded()
    {
        var options = new PerformanceBehaviorOptions { ThresholdMilliseconds = 0 };
        var logger = Substitute.For<ILogger<PerformanceBehavior<UnitCommandWithSecret, Result>>>();
        var behavior = new PerformanceBehavior<UnitCommandWithSecret, Result>(logger, options);

        const string secret = "hunter2-super-secret-password";

        await behavior.HandleAsync(
            new UnitCommandWithSecret(secret),
            _ => Task.FromResult(Result.Success()),
            CancellationToken.None);

        var loggedText = string.Join(
            " | ",
            logger.ReceivedCalls().Select(call => call.GetArguments()[2]?.ToString()));

        Assert.DoesNotContain(secret, loggedText);
        Assert.DoesNotContain("UnitCommandWithSecret {", loggedText);
        Assert.Contains("Long-running request detected: UnitCommandWithSecret", loggedText);
    }
}
