namespace CM.Domain.Cqrs;

/// <summary>
/// Sends messages of type <typeparamref name="TMessage"/>.
/// </summary>
/// <typeparam name="TMessage">The type of message to send.</typeparam>
public interface IMessenger<TMessage>
{
    /// <summary>
    /// Sends the specified message asynchronously.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendAsync(TMessage message, CancellationToken cancellationToken = default);
}
