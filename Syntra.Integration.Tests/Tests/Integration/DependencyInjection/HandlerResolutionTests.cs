using Microsoft.Extensions.DependencyInjection;
using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Results;
using Syntra.DependencyInjection.Registration;
using Syntra.DependencyInjection.Scanning;

namespace Syntra.Integration.Tests.Tests.Integration.DependencyInjection;

public sealed class HandlerResolutionTests
{
    [Fact]
    public void HandlerConvention_includes_integration_query_handler()
    {
        Assert.True(HandlerConventions.IsRequestHandler(typeof(IntegPingQueryHandler)));
    }

    [Fact]
    public void Scrutor_registers_concrete_handler_interfaces()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSyntra(c => c
            .AddBehaviors(_ => { })
            .ScanAssemblies(typeof(IntegPingQuery).Assembly)
            .ValidateDependencies(false));

        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<IntegPingQuery, Result<string>>>();

        Assert.IsType<IntegPingQueryHandler>(handler);
    }
}
