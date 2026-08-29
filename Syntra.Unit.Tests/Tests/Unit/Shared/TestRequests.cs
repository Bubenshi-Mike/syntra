using Syntra.Abstractions.Notifications;
using Syntra.Abstractions.Requests;

namespace Syntra.Unit.Tests.Tests.Unit.Shared;

internal sealed record UnitPingQuery : IQuery<int>;

internal sealed record UnitEmptyCommand : ICommand;

internal sealed record UnitStreamNumbersQuery : IStreamQuery<int>;

internal sealed record UnitTestNotification : INotification;
