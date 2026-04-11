using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Mediator;
using Syntra.Abstractions.Requests;
using Syntra.Abstractions.Results;
using Syntra.DependencyInjection.Registration;

namespace Syntra.Benchmarks;

[MemoryDiagnoser]
public class MediatorBenchmarks
{
    private ServiceProvider _provider = null!;
    private IServiceScope _scope = null!;
    private ISyntraMediator _mediator = null!;
    private BenchPing _request = null!;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSyntra(c => c.AddBehaviors(_ => { }));
        services.AddSingleton<IRequestHandler<BenchPing, Result<int>>, BenchPingHandler>();
        _provider = services.BuildServiceProvider();
        _scope = _provider.CreateScope();
        _mediator = _scope.ServiceProvider.GetRequiredService<ISyntraMediator>();
        _request = new BenchPing();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _scope.Dispose();
        _provider.Dispose();
    }

    [Benchmark]
    public async Task<int> Send_ping_query()
    {
        var r = await _mediator.SendAsync(_request).ConfigureAwait(false);
        return r.IsSuccess ? r.Value : 0;
    }

    public sealed record BenchPing : IQuery<int>;

    public sealed class BenchPingHandler : IQueryHandler<BenchPing, int>
    {
        public Task<Result<int>> HandleAsync(BenchPing request, CancellationToken cancellationToken = default) =>
            Task.FromResult(Result.Success(1));
    }
}
