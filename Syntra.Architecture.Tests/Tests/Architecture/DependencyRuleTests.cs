using NetArchTest.Rules;
using Syntra.Abstractions.Requests;

namespace Syntra.Architecture.Tests.Tests.Architecture;

public sealed class DependencyRuleTests
{
    [Fact]
    public void Abstractions_does_not_depend_on_Microsoft_Extensions_DependencyInjection()
    {
        var result = Types.InAssembly(typeof(IRequest).Assembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.Extensions.DependencyInjection")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Behaviors_does_not_reference_DependencyInjection_assembly()
    {
        var behaviors = typeof(Syntra.Behaviors.Logging.LoggingBehavior<,>).Assembly;
        var infra = typeof(Syntra.DependencyInjection.Registration.SyntraServiceCollectionExtensions).Assembly;

        Assert.DoesNotContain(infra.GetName().Name, behaviors.GetReferencedAssemblies().Select(a => a.Name));
    }

    [Fact]
    public void Core_Syntra_references_Abstractions()
    {
        var core = typeof(Syntra.Options.SyntraMediatorOptions).Assembly;
        var abstractions = typeof(IRequest).Assembly;

        Assert.Contains(abstractions.GetName().Name, core.GetReferencedAssemblies().Select(a => a.Name));
    }

    // Previously there were two IStreamQuery<T> interfaces in different namespaces (Requests and
    // Streams), which doubled the public API surface for one concept and let SYN001/SYN010's
    // metadata-name-based matching confuse the two. Guards against that reappearing.
    [Fact]
    public void Abstractions_declares_exactly_one_IStreamQuery_interface()
    {
        var streamQueryTypes = typeof(IRequest).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsGenericTypeDefinition && t.Name == "IStreamQuery`1")
            .ToList();

        Assert.Single(streamQueryTypes);
    }
}
