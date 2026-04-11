using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Syntra.DependencyInjection.Configuration;
using Syntra.DependencyInjection.Dependencies;
using Syntra.Mediator;
using Syntra.Notifications;
using Syntra.Options;

namespace Syntra.DependencyInjection.Registration;

/// <summary>
/// Fluent configuration for Syntra mediator, behaviors, scanning, and root <see cref="SyntraOptions"/>.
/// </summary>
public sealed class SyntraConfigurationBuilder
{
    private readonly IServiceCollection _services;
    private IConfiguration? _configuration;
    private readonly List<Assembly> _assemblies = new();
    private Action<SyntraOptions>? _configureRoot;
    private BehaviorRegistrationBuilder? _behaviors;
    private bool _validateDependencies = true;

    internal SyntraConfigurationBuilder(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>The underlying service collection.</summary>
    public IServiceCollection Services => _services;

    /// <summary>Optional configuration binding for <see cref="SyntraOptions"/>.</summary>
    public SyntraConfigurationBuilder UseConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
        return this;
    }

    /// <summary>Registers pipeline behaviors (order = outermost first).</summary>
    public SyntraConfigurationBuilder AddBehaviors(Action<BehaviorRegistrationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _behaviors = new BehaviorRegistrationBuilder();
        configure(_behaviors);
        return this;
    }

    /// <summary>Scrutor-based handler and support discovery.</summary>
    public SyntraConfigurationBuilder ScanAssemblies(params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);
        _assemblies.AddRange(assemblies);
        return this;
    }

    /// <summary>Inline merge into <see cref="SyntraOptions"/> (runs after configuration binding).</summary>
    public SyntraConfigurationBuilder Configure(Action<SyntraOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _configureRoot = _configureRoot is null
            ? configure
            : _configureRoot + configure;
        return this;
    }

    /// <summary>When <c>false</c>, skips <see cref="SyntraDependencyValidator"/> after registration.</summary>
    public SyntraConfigurationBuilder ValidateDependencies(bool validate = true)
    {
        _validateDependencies = validate;
        return this;
    }

    /// <summary>Manual registration (handlers, behaviors) without the fluent pipeline.</summary>
    public SyntraBuilder Manual => new(_services);

    internal void Complete()
    {
        if (_configuration is not null)
            _services.Configure<SyntraOptions>(_configuration.GetSection(SyntraOptions.SectionName));
        else
            _services.Configure<SyntraOptions>(_ => { });

        if (_configureRoot is not null)
            _services.PostConfigure(_configureRoot);

        SyntraConfigurationBinder.RegisterOptionBridges(_services);

        RegisterCoreMediator(_services);

        if (_behaviors is not null)
            BehaviorRegistrar.Apply(_services, _behaviors);

        if (_assemblies.Count > 0)
            HandlerRegistrar.Register(_services, _assemblies);

        if (_validateDependencies)
            SyntraDependencyValidator.Validate(_services);
    }

    private static void RegisterCoreMediator(IServiceCollection services)
    {
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
    }
}
