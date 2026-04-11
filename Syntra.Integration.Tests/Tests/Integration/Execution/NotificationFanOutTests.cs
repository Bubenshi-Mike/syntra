using Microsoft.Extensions.DependencyInjection;
using Syntra.Abstractions.Mediator;
using Syntra.DependencyInjection.Registration;

namespace Syntra.Integration.Tests.Tests.Integration.Execution;

public sealed class NotificationFanOutTests
{
    [Fact]
    public async Task PublishAsync_invokes_all_handlers()
    {
        IntegEventHandler.CallCount = 0;
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSyntra(c => c
            .AddBehaviors(_ => { })
            .ScanAssemblies(typeof(IntegEvent).Assembly)
            .ValidateDependencies(false));

        await using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISyntraMediator>();

        await mediator.PublishAsync(new IntegEvent()).ConfigureAwait(false);

        Assert.Equal(1, IntegEventHandler.CallCount);
    }
}
