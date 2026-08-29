using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Syntra.Abstractions.Results;
using Syntra.Behaviors.Audit;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class AuditBehaviorTests
{
    [Fact]
    public async Task Non_auditable_request_skips_audit_writer()
    {
        var writer = Substitute.For<IAuditWriter>();
        var behavior = new AuditBehavior<UnitPingQuery, Result<int>>(
            writer,
            NullLogger<AuditBehavior<UnitPingQuery, Result<int>>>.Instance);

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(0)),
            CancellationToken.None);

        Assert.True(response.IsSuccess);
        await writer.DidNotReceive().WriteAsync(Arg.Any<AuditEntry>(), Arg.Any<CancellationToken>());
    }
}
