using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Syntra.Abstractions.Results;
using Syntra.Behaviors.Validation;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task With_no_validators_delegates_to_next()
    {
        IEnumerable<IValidator<UnitPingQuery>> validators = Array.Empty<IValidator<UnitPingQuery>>();
        var behavior = new ValidationBehavior<UnitPingQuery, Result<int>>(
            validators,
            NullLogger<ValidationBehavior<UnitPingQuery, Result<int>>>.Instance);

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(9)),
            CancellationToken.None).ConfigureAwait(false);

        Assert.True(response.IsSuccess);
        Assert.Equal(9, response.Value);
    }
}
