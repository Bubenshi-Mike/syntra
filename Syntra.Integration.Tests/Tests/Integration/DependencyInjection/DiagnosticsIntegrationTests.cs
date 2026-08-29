using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Syntra.Abstractions.Mediator;
using Syntra.DependencyInjection.Registration;

namespace Syntra.Integration.Tests.Tests.Integration.DependencyInjection;

public sealed class DiagnosticsIntegrationTests
{
    [Fact]
    public async Task AddDiagnostics_records_a_tracing_activity_and_request_metrics()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSyntra(c => c
            .AddBehaviors(b => b.AddDiagnostics())
            .ScanAssemblies(typeof(IntegPingQuery).Assembly)
            .ValidateDependencies(false));

        await using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISyntraMediator>();

        var recordedActivities = new List<Activity>();
        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Syntra",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStopped = recordedActivities.Add,
        };
        ActivitySource.AddActivityListener(activityListener);

        var recordedDurations = new List<double>();
        using var meterListener = new MeterListener();
        meterListener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Meter.Name == "Syntra" && instrument.Name == "syntra.requests.duration")
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };
        meterListener.SetMeasurementEventCallback<double>((_, measurement, _, _) => recordedDurations.Add(measurement));
        meterListener.Start();

        var result = await mediator.SendAsync(new IntegPingQuery());

        Assert.True(result.IsSuccess);

        var activity = Assert.Single(recordedActivities);
        Assert.Equal("syntra.mediator.request", activity.OperationName);
        Assert.Equal(
            typeof(IntegPingQuery).FullName,
            activity.Tags.FirstOrDefault(t => t.Key == "syntra.request.type").Value);

        Assert.Single(recordedDurations);
    }
}
