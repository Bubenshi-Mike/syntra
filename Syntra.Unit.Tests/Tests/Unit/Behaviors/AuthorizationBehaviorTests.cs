using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Syntra.Abstractions.Results;
using Syntra.Behaviors.Authorization;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Behaviors;

public sealed class AuthorizationBehaviorTests
{
    [Fact]
    public async Task Non_authorized_request_skips_auth_pipeline()
    {
        var http = Substitute.For<IHttpContextAccessor>();
        var authorizationService = Substitute.For<IAuthorizationService>();
        var options = new AuthorizationBehaviorOptions { FailWhenHttpContextMissing = false };
        var behavior = new AuthorizationBehavior<UnitPingQuery, Result<int>>(
            http,
            authorizationService,
            options,
            NullLogger<AuthorizationBehavior<UnitPingQuery, Result<int>>>.Instance);

        var response = await behavior.HandleAsync(
            new UnitPingQuery(),
            _ => Task.FromResult(Result.Success(11)),
            CancellationToken.None).ConfigureAwait(false);

        Assert.True(response.IsSuccess);
        Assert.Equal(11, response.Value);
    }
}
