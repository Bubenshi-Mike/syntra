using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Results;

namespace Syntra.ConsoleApp.Demo;

public sealed class FlakyEchoQueryHandler(ILogger<FlakyEchoQueryHandler> logger) : IQueryHandler<FlakyEchoQuery, string>
{
    private static int _attempts;

    public Task<Result<string>> HandleAsync(FlakyEchoQuery request, CancellationToken cancellationToken = default)
    {
        var n = Interlocked.Increment(ref _attempts);
        if (n % 3 != 0)
        {
            logger.LogDebug("FlakyEcho attempt {Attempt} — simulating transient failure", n);
            throw new InvalidOperationException("Transient downstream failure (demo).");
        }

        return Task.FromResult(Result.Success($"echo:{request.Text}"));
    }
}
