using Syntra.Abstractions.Notifications;
using Syntra.Notifications;

namespace Syntra.Unit.Tests.Tests.Unit.Notifications;

public sealed class NotificationPublisherTests
{
    [Fact]
    public async Task Sequential_publisher_invokes_handlers_in_order()
    {
        var order = new List<int>();
        var publisher = new SequentialNotificationPublisher();
        INotificationHandler<TestNotif>[] handlers =
        [
            new DelegatingHandler(_ => order.Add(1)),
            new DelegatingHandler(_ => order.Add(2)),
        ];

        await publisher.PublishAsync(handlers, new TestNotif(), CancellationToken.None);

        Assert.Equal(new[] { 1, 2 }, order);
    }

    private sealed record TestNotif : INotification;

    private sealed class DelegatingHandler : INotificationHandler<TestNotif>
    {
        private readonly Action<TestNotif> _on;

        public DelegatingHandler(Action<TestNotif> on) => _on = on;

        public Task HandleAsync(TestNotif notification, CancellationToken cancellationToken = default)
        {
            _on(notification);
            return Task.CompletedTask;
        }
    }
}
