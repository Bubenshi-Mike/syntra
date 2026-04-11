using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Syntra.Abstractions.Handlers;
using Syntra.DependencyInjection.Registration;
using Syntra.Abstractions.Mediator;
using Syntra.Unit.Tests.Tests.Unit.Shared;

namespace Syntra.Unit.Tests.Tests.Unit.Mediator;

public sealed class SyntraMediatorStreamTests
{
    [Fact]
    public async Task CreateStreamAsync_yields_items_from_stream_handler()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSyntra(static (SyntraConfigurationBuilder _) => { });
        services.AddScoped<IStreamQueryHandler<UnitStreamNumbersQuery, int>, UnitStreamNumbersQueryHandler>();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISyntraMediator>();

        var items = new List<int>();
        await foreach (var n in mediator.CreateStreamAsync(new UnitStreamNumbersQuery()).ConfigureAwait(false))
            items.Add(n);

        Assert.Equal(new[] { 1, 2 }, items);
    }
}
