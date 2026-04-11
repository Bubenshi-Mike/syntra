using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Syntra.Abstractions.Mediator;
using Syntra.DependencyInjection.Registration;
using Syntra.Abstractions.Notifications;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Mediator;

public sealed class SyntraMediatorPublishTests
{
    [Fact]
    public async Task PublishAsync_invokes_notification_handler()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSyntra(static (SyntraConfigurationBuilder _) => { });
        services.AddScoped<INotificationHandler<UnitTestNotification>, UnitTestNotificationHandler>();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISyntraMediator>();

        await mediator.PublishAsync(new UnitTestNotification()).ConfigureAwait(false);
    }
}
