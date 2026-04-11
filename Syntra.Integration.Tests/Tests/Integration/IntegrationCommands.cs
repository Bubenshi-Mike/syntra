using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Notifications;
using Syntra.Abstractions.Requests;
using Syntra.Abstractions.Results;

namespace Syntra.Integration.Tests.Tests.Integration;

public sealed record IntegPingQuery : IQuery<string>;

public sealed class IntegPingQueryHandler : IQueryHandler<IntegPingQuery, string>
{
    public Task<Result<string>> HandleAsync(IntegPingQuery request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success("ok"));
}

public sealed record IntegEvent : INotification;

public sealed class IntegEventHandler : INotificationHandler<IntegEvent>
{
    public static int CallCount;

    public Task HandleAsync(IntegEvent notification, CancellationToken cancellationToken = default)
    {
        CallCount++;
        return Task.CompletedTask;
    }
}
