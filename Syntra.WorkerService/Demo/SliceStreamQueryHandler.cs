using Syntra.Abstractions.Handlers;

namespace Syntra.WorkerService.Demo;

public sealed class SliceStreamQueryHandler : IStreamQueryHandler<SliceStreamQuery, string>
{
    public async IAsyncEnumerable<string> HandleAsync(
        SliceStreamQuery query,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        for (var i = 0; i < query.Count; i++)
        {
            await Task.Delay(50, cancellationToken).ConfigureAwait(false);
            yield return $"slice-{i + 1}/{query.Count}";
        }
    }
}
