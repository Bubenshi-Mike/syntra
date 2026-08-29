namespace Syntra.Abstractions.Handlers;

/// <summary>
/// Handles a command that has no return value beyond success/failure.
/// </summary>
/// <typeparam name="TCommand">
/// The command type. Must implement <see cref="ICommand"/>.
/// </typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
    // Inherits: Task<Result> HandleAsync(TCommand request, CancellationToken cancellationToken)
}

/// <summary>
/// Handles a command that returns a value on success.
/// </summary>
/// <typeparam name="TCommand">
/// The command type. Must implement <see cref="ICommand{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The value produced on success (e.g. the ID of a created entity).
/// </typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
    // Inherits: Task<Result<TResponse>> HandleAsync(TCommand request, CancellationToken cancellationToken)
}
