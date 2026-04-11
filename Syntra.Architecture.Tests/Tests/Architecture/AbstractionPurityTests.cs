using NetArchTest.Rules;
using Syntra.Abstractions.Requests;

namespace Syntra.Architecture.Tests.Tests.Architecture;

public sealed class AbstractionPurityTests
{
    [Fact]
    public void Abstractions_has_no_dependency_on_dependency_injection_namespace()
    {
        var result = Types.InAssembly(typeof(IRequest).Assembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.Extensions.DependencyInjection")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Abstractions_public_surface_has_no_ServiceCollection_type()
    {
        var asm = typeof(IRequest).Assembly;
        foreach (var type in asm.GetExportedTypes())
        {
            Assert.NotEqual("ServiceCollection", type.Name);
        }
    }
}
