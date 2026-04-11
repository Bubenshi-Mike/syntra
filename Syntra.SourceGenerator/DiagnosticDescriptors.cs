using Microsoft.CodeAnalysis;

namespace Syntra.SourceGenerator;

internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor RequestHasNoHandler = new(
        id: "SYN101",
        title: "Request type has no registered handler",
        messageFormat: "No IRequestHandler<,> implementation was found for request type '{0}'",
        category: "Syntra",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MultipleHandlersForRequest = new(
        id: "SYN102",
        title: "Multiple handlers for the same request type",
        messageFormat: "More than one IRequestHandler<,> targets request type '{0}'",
        category: "Syntra",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor HandlerMissingCancellationToken = new(
        id: "SYN103",
        title: "Handler HandleAsync should accept CancellationToken",
        messageFormat: "Type '{0}' implements a handler interface but HandleAsync does not include CancellationToken as the last parameter",
        category: "Syntra",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor CommandDoesNotImplementICommand = new(
        id: "SYN104",
        title: "Command handler command type should implement ICommand",
        messageFormat: "Type '{0}' is used with ICommandHandler<> but does not implement ICommand or ICommand<T>",
        category: "Syntra",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
