namespace CM.Domain.Cqrs;

/// <summary>
/// Non-generic interface for accessing <see cref="Result{T}"/> values without reflection.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets the error result when this instance represents a failure.
    /// </summary>
    FailResult Error { get; }

    /// <summary>
    /// Gets a value indicating whether this instance represents a failure.
    /// </summary>
    bool IsError { get; }

    /// <summary>
    /// Gets a value indicating whether this instance represents a success.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the success value, or <see langword="null"/> when this instance represents a failure.
    /// </summary>
    object? Value { get; }
}
