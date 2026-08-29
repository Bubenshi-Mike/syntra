using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Syntra.DependencyInjection.Configuration;
using Syntra.Mediator;
using Syntra.Notifications;
using Syntra.Options;

namespace Syntra.DependencyInjection.Registration;

/// <summary>
/// Primary DI entry point for Syntra.
/// </summary>
public static class SyntraServiceCollectionExtensions
{
    /// <summary>
    /// Registers Syntra with fluent configuration (behaviors, scanning, <see cref="SyntraOptions"/>).
    /// </summary>
    public static IServiceCollection AddSyntra(
        this IServiceCollection services,
        Action<SyntraConfigurationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new SyntraConfigurationBuilder(services);
        configure(builder);
        builder.Complete();
        return services;
    }

    /// <summary>
    /// Registers Syntra with configuration binding from <paramref name="configuration"/> (section <see cref="SyntraOptions.SectionName"/>).
    /// </summary>
    public static IServiceCollection AddSyntra(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<SyntraConfigurationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new SyntraConfigurationBuilder(services).UseConfiguration(configuration);
        configure(builder);
        builder.Complete();
        return services;
    }

    /// <summary>
    /// Minimal registration: mediator + notification publisher only (no behaviors, no <see cref="SyntraOptions"/> root).
    /// Prefer <see cref="AddSyntra(IServiceCollection, Action{SyntraConfigurationBuilder})"/> for full setup.
    /// </summary>
    public static SyntraBuilder AddSyntra(this IServiceCollection services, Action<SyntraMediatorOptions> configureMediator)
    {
        ArgumentNullException.ThrowIfNull(configureMediator);
        services.Configure(configureMediator);

        services.TryAddSingleton<INotificationPublisher>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<SyntraMediatorOptions>>().Value;
            return opts.NotificationDispatch switch
            {
                NotificationDispatchStrategy.ParallelWhenAll => new ParallelWhenAllNotificationPublisher(),
                NotificationDispatchStrategy.ParallelFireAndForget => new ParallelNoWaitNotificationPublisher(),
                _ => new SequentialNotificationPublisher(),
            };
        });
        services.TryAddScoped<ISyntraMediator, SyntraMediator>();

        return new SyntraBuilder(services);
    }
}
