using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Syntra.Abstractions.Results;
using Syntra.Behaviors.Transactions;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class TransactionBehaviorTests
{
    [Fact]
    public async Task Non_transactional_request_skips_transaction_manager()
    {
        var tm = Substitute.For<ITransactionManager>();
        var behavior = new TransactionBehavior<UnitPingQuery, Result<int>>(
            tm,
            NullLogger<TransactionBehavior<UnitPingQuery, Result<int>>>.Instance);

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(6)),
            CancellationToken.None);

        Assert.True(response.IsSuccess);
        await tm.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }
}
