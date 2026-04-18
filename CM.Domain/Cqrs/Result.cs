namespace CM.Domain.Cqrs;

/// <summary>
/// Represents the result of an operation as a discriminated union of a success value of type
/// <typeparamref name="T"/> or an <see cref="ErrorResult"/>.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public sealed class Result<T>
{
    private readonly ErrorResult? _error;
    private readonly T? _value;

    private Result(ErrorResult error)
    {
        _error = error;
        IsSuccess = false;
    }

    private Result(T value)
    {
        _value = value;
        IsSuccess = true;
    }

    /// <summary>
    /// Gets the error result when this instance represents a failure.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the result is a success.</exception>
    public ErrorResult Error => !IsSuccess
        ? _error!
        : throw new InvalidOperationException("Result is a success. Access Value instead.");

    /// <summary>
    /// Gets a value indicating whether this instance represents a failure.
    /// </summary>
    public bool IsError => !IsSuccess;

    /// <summary>
    /// Gets a value indicating whether this instance represents a success.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the success value when this instance represents a success.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the result is a failure.</exception>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Result is an error. Access Error instead.");

    /// <summary>
    /// Implicitly converts an <see cref="ErrorResult"/> to a failed <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="error">The error result.</param>
    public static implicit operator Result<T>(ErrorResult error) => new(error);

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="T"/> to a successful <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">The success value.</param>
    public static implicit operator Result<T>(T value) => new(value);
}
