using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Syntra.Abstractions.Handlers;
using Syntra.DependencyInjection.Registration;
using Syntra.Abstractions.Mediator;
using Syntra.Abstractions.Results;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Mediator;

public sealed class SyntraMediatorSendTests
{
    [Fact]
    public async Task SendAsync_resolves_handler_and_returns_result()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSyntra(static (SyntraConfigurationBuilder _) => { });
        services.AddScoped<IRequestHandler<UnitPingQuery, Result<int>>, UnitPingQueryHandler>();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISyntraMediator>();

        var response = await mediator.SendAsync(new UnitPingQuery()).ConfigureAwait(false);

        Assert.True(response.IsSuccess);
        Assert.Equal(42, response.Value);
    }
}
