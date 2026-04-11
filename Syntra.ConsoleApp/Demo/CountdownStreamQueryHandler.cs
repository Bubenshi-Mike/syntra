using Syntra.Abstractions.Handlers;

namespace Syntra.ConsoleApp.Demo;

public sealed class CountdownStreamQueryHandler(ILogger<CountdownStreamQueryHandler> logger)
    : IStreamQueryHandler<CountdownStreamQuery, int>
{
    public async IAsyncEnumerable<int> HandleAsync(
        CountdownStreamQuery query,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (query.Start <= query.End)
        {
            for (var i = query.Start; i <= query.End; i++)
            {
                logger.LogDebug("Stream tick {Value}", i);
                await Task.Delay(30, cancellationToken).ConfigureAwait(false);
                yield return i;
            }
        }
        else
        {
            for (var i = query.Start; i >= query.End; i--)
            {
                logger.LogDebug("Stream tick {Value}", i);
                await Task.Delay(30, cancellationToken).ConfigureAwait(false);
                yield return i;
            }
        }
    }
}
