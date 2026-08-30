using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
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
            CancellationToken.None);

        Assert.True(response.IsSuccess);
    }

    // Regression test for a real leak: {@Request}/record ToString() destructuring previously
    // logged the entire request object, including sensitive fields (e.g. a Password property),
    // on every single request at Information level. Syntra has no way to know which properties
    // on an arbitrary, consumer-defined request type are sensitive, so the fix is to never log
    // the request object's contents at all - only its type name.
    [Fact]
    public async Task Does_not_log_the_raw_request_object_or_sensitive_field_values()
    {
        var logger = Substitute.For<ILogger<LoggingBehavior<UnitCommandWithSecret, Result>>>();
        var behavior = new LoggingBehavior<UnitCommandWithSecret, Result>(logger);

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
        Assert.Contains("Handling UnitCommandWithSecret", loggedText);
    }
}
