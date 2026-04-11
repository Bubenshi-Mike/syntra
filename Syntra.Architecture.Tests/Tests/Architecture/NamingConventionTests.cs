using System.Linq;
using Syntra.Abstractions.Pipelines;

namespace Syntra.Architecture.Tests.Tests.Architecture;

public sealed class NamingConventionTests
{
    [Fact]
    public void Behavior_types_in_Behaviors_assembly_end_with_Behavior()
    {
        var asm = typeof(Syntra.Behaviors.Logging.LoggingBehavior<,>).Assembly;
        foreach (var type in asm.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract)
                continue;
            if (!type.GetInterfaces().Any(IsPipelineBehavior))
                continue;

            var simpleName = type.Name.Split('`')[0];
            Assert.EndsWith("Behavior", simpleName, StringComparison.Ordinal);
        }
    }

    private static bool IsPipelineBehavior(Type i) =>
        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>);
}
