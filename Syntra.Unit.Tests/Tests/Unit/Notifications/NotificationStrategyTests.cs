using Syntra.Abstractions.Notifications;
using Syntra.Options;

namespace Syntra.Unit.Tests.Tests.Unit.Notifications;

public sealed class NotificationStrategyTests
{
    [Fact]
    public void Default_mediator_options_use_sequential_dispatch()
    {
        var options = new SyntraMediatorOptions();
        Assert.Equal(NotificationDispatchStrategy.Sequential, options.NotificationDispatch);
    }
}
