namespace CM.Domain.Cqrs;

/// <summary>
/// Indicates that a type can validate its own state.
/// </summary>
public interface IValidatable
{
    /// <summary>
    /// Validates the current instance.
    /// </summary>
    /// <returns>A <see cref="FailResult"/> describing the validation failure, or <see langword="null"/> if valid.</returns>
    FailResult? Validate();
}
