namespace CM.Domain.Cqrs;

/// <summary>
/// Represents an error that occurred during an operation.
/// </summary>
public sealed class FailResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailResult"/> class.
    /// </summary>
    /// <param name="message">The message describing the error.</param>
    public FailResult(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Gets the message describing the error.
    /// </summary>
    public string Message { get; init; }
}
