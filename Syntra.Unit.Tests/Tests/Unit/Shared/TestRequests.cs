using Syntra.Abstractions.Notifications;
using Syntra.Abstractions.Requests;

namespace Syntra.Unit.Tests.Tests.Unit.Shared;

internal sealed record UnitPingQuery : IQuery<int>;

internal sealed record UnitEmptyCommand : ICommand;

internal sealed record UnitStreamNumbersQuery : IStreamQuery<int>;

internal sealed record UnitTestNotification : INotification;

// Public (unlike the other test requests above) because NSubstitute's Castle proxy needs to
// close ILogger<SomeBehavior<UnitCommandWithSecret, ...>> around it, which Castle can't do for
// an internal type without an InternalsVisibleTo grant to its dynamic proxy assembly.
public sealed record UnitCommandWithSecret(string Password) : ICommand;
