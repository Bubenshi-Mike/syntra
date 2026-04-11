namespace Syntra.SourceGenerator;

/// <summary>
/// Legacy ISyntaxReceiver pattern is superseded by <see cref="Microsoft.CodeAnalysis.IIncrementalGenerator"/> in <see cref="SyntraHandlerGenerator"/>.
/// This type is retained for documentation and future ISyntaxContextReceiver experiments.
/// </summary>
internal static class HandlerSyntaxReceiverNotes
{
    // Incremental generators use SyntaxProvider.CreateSyntaxProvider instead of a syntax receiver.
}
