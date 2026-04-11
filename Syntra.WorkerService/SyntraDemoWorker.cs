using Syntra.Abstractions.Mediator;
using Syntra.WorkerService.Demo;

namespace Syntra.WorkerService;

/// <summary>
/// Resolves a scoped <see cref="ISyntraMediator"/> per iteration — mirrors real worker usage
/// (database scopes, per-message isolation).
/// </summary>
public sealed class SyntraDemoWorker(IServiceScopeFactory scopeFactory, ILogger<SyntraDemoWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Syntra worker started — publishing notifications and streaming on an interval.");

        var tick = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var mediator = scope.ServiceProvider.GetRequiredService<ISyntraMediator>();

            tick++;
            await mediator.PublishAsync(new HeartbeatNotification(DateTimeOffset.UtcNow, tick), stoppingToken)
                .ConfigureAwait(false);

            logger.LogInformation("Streaming slice batch (iteration {Tick})", tick);
            await foreach (var line in mediator.CreateStreamAsync(new SliceStreamQuery(4), stoppingToken)
                               .ConfigureAwait(false))
                logger.LogInformation("  stream: {Line}", line);

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(6), stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }
}
