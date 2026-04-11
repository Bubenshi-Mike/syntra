using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Syntra.Abstractions.Mediator;
using Syntra.Abstractions.Results;
using Syntra.DependencyInjection.Registration;

namespace Syntra.Integration.Tests.Tests.Integration.Behaviors;

public sealed class RetryIntegrationTests
{
    [Fact]
    public async Task Non_retryable_request_dispatches_normally()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSyntra(c => c
            .AddBehaviors(b => b.AddRetry())
            .ScanAssemblies(typeof(IntegPingQuery).Assembly)
            .ValidateDependencies(false));

        await using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISyntraMediator>();

        var r = await mediator.SendAsync(new IntegPingQuery()).ConfigureAwait(false);
        Assert.True(r.IsSuccess);
    }
}
