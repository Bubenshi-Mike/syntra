using Microsoft.Extensions.DependencyInjection;
using Syntra.Abstractions.Mediator;
using Syntra.DependencyInjection.Registration;

namespace Syntra.Integration.Tests.Tests.Integration.Execution;

public sealed class CommandDispatchTests
{
    [Fact]
    public async Task SendAsync_dispatches_through_mediator()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSyntra(c => c
            .AddBehaviors(_ => { })
            .ScanAssemblies(typeof(IntegPingQuery).Assembly)
            .ValidateDependencies(false));

        await using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISyntraMediator>();

        var result = await mediator.SendAsync(new IntegPingQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal("ok", result.Value);
    }
}
