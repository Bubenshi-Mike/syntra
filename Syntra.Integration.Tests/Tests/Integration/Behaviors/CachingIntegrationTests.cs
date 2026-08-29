using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Syntra.Abstractions.Mediator;
using Syntra.DependencyInjection.Registration;

namespace Syntra.Integration.Tests.Tests.Integration.Behaviors;

public sealed class CachingIntegrationTests
{
    [Fact]
    public async Task Caching_pipeline_resolves_with_distributed_cache()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSingleton<IDistributedCache>(_ => Substitute.For<IDistributedCache>());
        services.AddSyntra(c => c
            .AddBehaviors(b => b.AddCaching())
            .ScanAssemblies(typeof(IntegPingQuery).Assembly)
            .ValidateDependencies(false));

        await using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISyntraMediator>();

        var r = await mediator.SendAsync(new IntegPingQuery());

        Assert.True(r.IsSuccess);
        Assert.Equal("ok", r.Value);
    }
}
