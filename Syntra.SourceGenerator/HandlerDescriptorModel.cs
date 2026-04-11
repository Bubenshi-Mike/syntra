using Microsoft.CodeAnalysis;

namespace Syntra.SourceGenerator;

internal sealed class HandlerDescriptorModel
{
    public string HandlerTypeName { get; set; } = "";
    public INamedTypeSymbol RequestTypeSymbol { get; set; } = null!;
    public Location PrimaryLocation { get; set; } = null!;
    public bool HandleAsyncHasCancellationToken { get; set; }
    public bool IsCommandHandler { get; set; }
}
