using Microsoft.CodeAnalysis;

namespace Syntra.Analyzers;

internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor RequestHasNoHandler = new(
        id: "SYN001",
        title: "Request type has no handler",
        messageFormat: "No IRequestHandler<,> or IStreamQueryHandler<,> implementation was found for '{0}'",
        category: "Syntra",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: new[] { WellKnownDiagnosticTags.CompilationEnd });

    public static readonly DiagnosticDescriptor DuplicateCommandHandlers = new(
        id: "SYN002",
        title: "Multiple command handlers for the same command",
        messageFormat: "More than one ICommandHandler<> targets command type '{0}'",
        category: "Syntra",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: new[] { WellKnownDiagnosticTags.CompilationEnd });

    public static readonly DiagnosticDescriptor HandlerMissingCancellationToken = new(
        id: "SYN003",
        title: "Handler HandleAsync should accept CancellationToken",
        messageFormat: "Type '{0}' implements a handler interface but HandleAsync does not include CancellationToken as the last parameter",
        category: "Syntra",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NamingConvention = new(
        id: "SYN010",
        title: "Syntra naming convention violation",
        messageFormat: "{0}",
        category: "Syntra",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor BehaviorRegistrationOrder = new(
        id: "SYN020",
        title: "Register behaviors before scanning assemblies",
        messageFormat: "Call AddBehaviors before ScanAssemblies in AddSyntra configuration so pipeline registration order is explicit",
        category: "Syntra",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor HandlerShouldNotThrow = new(
        id: "SYN030",
        title: "Prefer Result.Failure over throwing in handlers",
        messageFormat: "HandleAsync returns a Result type; use Result.Failure instead of throw for expected failures",
        category: "Syntra",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);
}
