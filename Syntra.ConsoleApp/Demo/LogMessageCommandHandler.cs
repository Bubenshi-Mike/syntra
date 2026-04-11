using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Results;

namespace Syntra.ConsoleApp.Demo;

public sealed class LogMessageCommandHandler(ILogger<LogMessageCommandHandler> logger) : ICommandHandler<LogMessageCommand>
{
    public Task<Result> HandleAsync(LogMessageCommand request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[COMMAND] {Message}", request.Message);
        return Task.FromResult(Result.Success());
    }
}
