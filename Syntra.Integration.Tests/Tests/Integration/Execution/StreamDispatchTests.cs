using Microsoft.Extensions.DependencyInjection;
using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Mediator;
using Syntra.Abstractions.Requests;
using Syntra.DependencyInjection.Registration;

namespace Syntra.Integration.Tests.Tests.Integration.Execution;

public sealed record IntegNumbersStream : IStreamQuery<int>;

public sealed class IntegNumbersStreamHandler : IStreamQueryHandler<IntegNumbersStream, int>
{
    public async IAsyncEnumerable<int> HandleAsync(
        IntegNumbersStream query,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        yield return 10;
    }
}

public sealed class StreamDispatchTests
{
    [Fact]
    public async Task CreateStreamAsync_iterates_handler_sequence()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSyntra(c => c
            .AddBehaviors(_ => { })
            .ScanAssemblies(typeof(IntegNumbersStream).Assembly)
            .ValidateDependencies(false));

        await using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISyntraMediator>();

        var list = new List<int>();
        await foreach (var n in mediator.CreateStreamAsync(new IntegNumbersStream()).ConfigureAwait(false))
        {
            list.Add(n);
        }

        Assert.Single(list);
        Assert.Equal(10, list[0]);
    }
}
