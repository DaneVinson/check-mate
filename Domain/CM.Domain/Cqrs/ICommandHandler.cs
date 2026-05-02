namespace CM.Domain.Cqrs;

/// <summary>
/// Non-generic base interface for command handlers, enabling dispatch without reflection.
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Handles the specified command asynchronously.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(object command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handles a command of type <typeparamref name="TCommand"/>.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
public interface ICommandHandler<TCommand> : ICommandHandler
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the specified command asynchronously.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
