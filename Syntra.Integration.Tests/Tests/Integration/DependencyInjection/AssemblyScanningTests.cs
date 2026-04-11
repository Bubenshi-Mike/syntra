using Microsoft.Extensions.DependencyInjection;
using Syntra.Abstractions.Notifications;
using Syntra.DependencyInjection.Registration;

namespace Syntra.Integration.Tests.Tests.Integration.DependencyInjection;

public sealed class AssemblyScanningTests
{
    [Fact]
    public void ScanAssemblies_finds_notification_handlers()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSyntra(c => c
            .AddBehaviors(_ => { })
            .ScanAssemblies(typeof(IntegEvent).Assembly)
            .ValidateDependencies(false));

        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<INotificationHandler<IntegEvent>>().ToArray();

        Assert.NotEmpty(handlers);
    }
}
